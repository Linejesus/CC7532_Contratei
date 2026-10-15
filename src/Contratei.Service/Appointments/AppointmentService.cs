using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Service.Appointments;

public sealed class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IProfessionalAvailabilityRepository _availabilityRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IContractorRepository _contractorRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IProfessionalAvailabilityRepository availabilityRepository,
        IProfessionalRepository professionalRepository,
        IContractorRepository contractorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _availabilityRepository = availabilityRepository;
        _professionalRepository = professionalRepository;
        _contractorRepository = contractorRepository;
    }

    public async Task<Appointment> CreateAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ProfessionalId == Guid.Empty ||
            request.ContractorId == Guid.Empty ||
            request.ProfessionalServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "Profissional, contratante e serviço são obrigatórios.");
        }

        if (request.StartAt <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException(
                "Não é possível agendar para um horário passado.");
        }

        var professional = await _professionalRepository.GetByIdAsync(
            request.ProfessionalId);

        if (professional is null)
        {
            throw new KeyNotFoundException(
                "Profissional não encontrado.");
        }

        var contractor = await _contractorRepository.GetByIdAsync(
            request.ContractorId, cancellationToken);

        if (contractor is null)
        {
            throw new KeyNotFoundException(
                "Contratante não encontrado.");
        }

        // O serviço precisa pertencer ao profissional e estar ativo.
        var professionalService = professional.Services.FirstOrDefault(
            service => service.Id == request.ProfessionalServiceId);

        if (professionalService is null)
        {
            throw new KeyNotFoundException(
                "Serviço não encontrado para este profissional.");
        }

        if (!professionalService.IsActive)
        {
            throw new InvalidOperationException(
                "O serviço selecionado está inativo.");
        }

        // O término é calculado com a duração cadastrada pelo profissional.
        var endAt = request.StartAt.Add(professionalService.Duration);

        var availabilities = await _availabilityRepository
            .GetByProfessionalIdAsync(
                request.ProfessionalId, cancellationToken);

        var activeAvailabilities = availabilities
            .Where(availability => availability.IsActive)
            .ToList();

        if (activeAvailabilities.Count == 0)
        {
            throw new InvalidOperationException(
                "O profissional ainda não configurou sua disponibilidade.");
        }

        if (!professional.Services.Any(service => service.IsActive))
        {
            throw new InvalidOperationException(
                "O profissional não possui serviços ativos.");
        }

        var matchingAvailability = activeAvailabilities.FirstOrDefault(
            availability => ContainsInterval(
                availability,
                request.StartAt,
                endAt));

        if (matchingAvailability is null)
        {
            throw new InvalidOperationException(
                "O horário solicitado está fora da disponibilidade " +
                "do profissional.");
        }

        var existingAppointments =
            await _appointmentRepository.GetByProfessionalAndPeriodAsync(
                request.ProfessionalId,
                request.StartAt,
                endAt,
                cancellationToken);

        var hasConflict = existingAppointments.Any(appointment =>
            appointment.Status is AppointmentStatus.Pending
                or AppointmentStatus.Confirmed);

        if (hasConflict)
        {
            throw new InvalidOperationException(
                "O horário solicitado já está ocupado.");
        }

        var appointment = new Appointment(
            request.ProfessionalId,
            request.ContractorId,
            professionalService.Id,
            request.StartAt,
            endAt,
            professionalService.Price);

        // Confirmação local após as validações.
        // A integração com o Google Calendar será feita posteriormente.
        appointment.Confirm();

        await _appointmentRepository.AddAsync(
            appointment, cancellationToken);

        return appointment;
    }

    public async Task<Appointment> GetByIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(
            appointmentId, cancellationToken);

        return appointment ?? throw new KeyNotFoundException(
            "Agendamento não encontrado.");
    }

    public async Task<IReadOnlyList<Appointment>> GetByContractorIdAsync(
        Guid contractorId,
        CancellationToken cancellationToken = default)
    {
        return await _appointmentRepository.GetByContractorIdAsync(
            contractorId, cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByProfessionalIdAsync(
        Guid professionalId,
        CancellationToken cancellationToken = default)
    {
        return await _appointmentRepository.GetByProfessionalIdAsync(
            professionalId, cancellationToken);
    }

    public async Task CancelAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(
            appointmentId, cancellationToken);

        if (appointment is null)
        {
            throw new KeyNotFoundException(
                "Agendamento não encontrado.");
        }

        appointment.Cancel();

        await _appointmentRepository.UpdateAsync(
            appointment, cancellationToken);
    }

    private static bool ContainsInterval(
        ProfessionalAvailability availability,
        DateTimeOffset startAt,
        DateTimeOffset endAt)
    {
        TimeZoneInfo timeZone;

        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                availability.TimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new InvalidOperationException(
                $"Fuso horário não encontrado: {availability.TimeZoneId}.");
        }
        catch (InvalidTimeZoneException)
        {
            throw new InvalidOperationException(
                $"Fuso horário inválido: {availability.TimeZoneId}.");
        }

        var localStart = TimeZoneInfo.ConvertTime(startAt, timeZone);
        var localEnd = TimeZoneInfo.ConvertTime(endAt, timeZone);

        if (DateOnly.FromDateTime(localStart.DateTime) !=
            DateOnly.FromDateTime(localEnd.DateTime))
        {
            return false;
        }

        if (localStart.DayOfWeek != availability.DayOfWeek)
        {
            return false;
        }

        return TimeOnly.FromDateTime(localStart.DateTime)
                   >= availability.StartTime
            && TimeOnly.FromDateTime(localEnd.DateTime)
                   <= availability.EndTime;
    }
}