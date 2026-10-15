using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;
using Contratei.Service.Appointments;

namespace Contratei.Service.Availability;

public sealed class ProfessionalAvailabilityService
{
    private readonly IProfessionalAvailabilityRepository _availabilityRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IProfessionalRepository _professionalRepository;

    public ProfessionalAvailabilityService(
        IProfessionalAvailabilityRepository availabilityRepository,
        IAppointmentRepository appointmentRepository,
        IProfessionalRepository professionalRepository)
    {
        _availabilityRepository = availabilityRepository;
        _appointmentRepository = appointmentRepository;
        _professionalRepository = professionalRepository;
    }

    public async Task<ProfessionalAvailability> ConfigureAsync(
        Guid professionalId,
        ConfigureAvailabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (professionalId == Guid.Empty)
            throw new ArgumentException("Profissional inválido.");

        var professional = await _professionalRepository.GetByIdAsync(
            professionalId);

        if (professional is null)
            throw new KeyNotFoundException("Profissional não encontrado.");

        ValidateTimeZone(request.TimeZoneId);

        var availability = new ProfessionalAvailability(
            professionalId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.TimeZoneId);

        var existing = await _availabilityRepository
            .GetByProfessionalIdAsync(professionalId, cancellationToken);

        var overlaps = existing.Any(item =>
            item.IsActive &&
            item.DayOfWeek == request.DayOfWeek &&
            request.StartTime < item.EndTime &&
            request.EndTime > item.StartTime);

        if (overlaps)
        {
            throw new InvalidOperationException(
                "Este intervalo se sobrepõe a uma disponibilidade existente.");
        }

        await _availabilityRepository.AddAsync(
            availability, cancellationToken);

        return availability;
    }

    public async Task<IReadOnlyList<ProfessionalAvailability>>
        GetWeeklyAvailabilityAsync(
            Guid professionalId,
            CancellationToken cancellationToken = default)
    {
        var professional = await _professionalRepository.GetByIdAsync(
            professionalId);

        if (professional is null)
            throw new KeyNotFoundException("Profissional não encontrado.");

        var availability = await _availabilityRepository
            .GetByProfessionalIdAsync(professionalId, cancellationToken);

        return availability
            .Where(item => item.IsActive)
            .OrderBy(item => item.DayOfWeek)
            .ThenBy(item => item.StartTime)
            .ToList();
    }

    public async Task<IReadOnlyList<AvailableTimeSlot>> GetAvailableSlotsAsync(
        Guid professionalId,
        DateOnly date,
        int durationMinutes,
        CancellationToken cancellationToken = default)
    {
        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                "A duração deve ser maior que zero.");

        var professional = await _professionalRepository.GetByIdAsync(
            professionalId);

        if (professional is null)
            throw new KeyNotFoundException("Profissional não encontrado.");

        var availabilities = await _availabilityRepository
            .GetByProfessionalIdAsync(professionalId, cancellationToken);

        var dayAvailabilities = availabilities
            .Where(a => a.IsActive && a.DayOfWeek == date.DayOfWeek)
            .OrderBy(a => a.StartTime)
            .ToList();

        if (dayAvailabilities.Count == 0)
            return Array.Empty<AvailableTimeSlot>();

        var dayStart = new DateTimeOffset(
            date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        var dayEnd = dayStart.AddDays(1);

        var appointments = await _appointmentRepository
            .GetByProfessionalAndPeriodAsync(
                professionalId,
                dayStart,
                dayEnd,
                cancellationToken);

        var blockingAppointments = appointments
            .Where(IsBlocking)
            .ToList();

        var slots = new List<AvailableTimeSlot>();

        foreach (var availability in dayAvailabilities)
        {
            var timeZone = FindTimeZone(availability.TimeZoneId);

            var localStart = date.ToDateTime(availability.StartTime);
            var localEnd = date.ToDateTime(availability.EndTime);

            if (timeZone.IsInvalidTime(localStart) ||
                timeZone.IsInvalidTime(localEnd))
            {
                continue;
            }

            if (timeZone.IsAmbiguousTime(localStart) ||
                timeZone.IsAmbiguousTime(localEnd))
            {
                // Evita escolher silenciosamente um offset em uma
                // transição de horário ambígua.
                continue;
            }

            var intervalStart = new DateTimeOffset(
                TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone));

            var intervalEnd = new DateTimeOffset(
                TimeZoneInfo.ConvertTimeToUtc(localEnd, timeZone));

            var candidate = intervalStart;

            while (candidate.AddMinutes(durationMinutes) <= intervalEnd)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var candidateEnd = candidate.AddMinutes(durationMinutes);

                var hasConflict = blockingAppointments.Any(a =>
                    candidate < a.EndAt && candidateEnd > a.StartAt);

                if (!hasConflict)
                {
                    slots.Add(new AvailableTimeSlot(candidate, candidateEnd));
                }

                candidate = candidateEnd;
            }
        }

        return slots
            .OrderBy(slot => slot.StartAt)
            .ToList();
    }

    public async Task DeactivateAsync(
        Guid availabilityId,
        CancellationToken cancellationToken = default)
    {
        var availability = await _availabilityRepository.GetByIdAsync(
            availabilityId, cancellationToken);

        if (availability is null)
            throw new KeyNotFoundException(
                "Disponibilidade não encontrada.");

        availability.Deactivate();

        await _availabilityRepository.UpdateAsync(
            availability, cancellationToken);
    }

    private static bool IsBlocking(Appointment appointment) =>
        appointment.Status is AppointmentStatus.Pending
            or AppointmentStatus.Confirmed;

    private static void ValidateTimeZone(string timeZoneId)
    {
        _ = FindTimeZone(timeZoneId);
    }

    private static TimeZoneInfo FindTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new ArgumentException(
                $"Fuso horário não encontrado: {timeZoneId}.");
        }
        catch (InvalidTimeZoneException)
        {
            throw new ArgumentException(
                $"Fuso horário inválido: {timeZoneId}.");
        }
    }
}