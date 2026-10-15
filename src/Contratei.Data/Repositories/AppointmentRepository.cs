using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly Dictionary<Guid, Appointment> _appointments = new();
    private readonly object _sync = new();

    public Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            _appointments.TryGetValue(id, out var appointment);
            return Task.FromResult(appointment);
        }
    }

    public Task<IReadOnlyList<Appointment>> GetByProfessionalAndPeriodAsync(
        Guid professionalId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException(
                "O profissional é obrigatório.",
                nameof(professionalId));
        }

        if (end <= start)
        {
            throw new ArgumentException(
                "O término do período deve ser posterior ao início.");
        }

        lock (_sync)
        {
            var appointments = _appointments.Values
                .Where(a => a.ProfessionalId == professionalId)
                .Where(a => a.StartAt < end && a.EndAt > start)
                .OrderBy(a => a.StartAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<Appointment>>(
                appointments);
        }
    }

    public Task<IReadOnlyList<Appointment>> GetByContractorIdAsync(
        Guid contractorId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (contractorId == Guid.Empty)
        {
            throw new ArgumentException(
                "O contratante é obrigatório.",
                nameof(contractorId));
        }

        lock (_sync)
        {
            var appointments = _appointments.Values
                .Where(a => a.ContractorId == contractorId)
                .OrderByDescending(a => a.StartAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<Appointment>>(
                appointments);
        }
    }

    public Task<IReadOnlyList<Appointment>> GetByProfessionalIdAsync(
        Guid professionalId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException(
                "O profissional é obrigatório.",
                nameof(professionalId));
        }

        lock (_sync)
        {
            var appointments = _appointments.Values
                .Where(a => a.ProfessionalId == professionalId)
                .OrderByDescending(a => a.StartAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<Appointment>>(
                appointments);
        }
    }

    public Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(appointment);

        lock (_sync)
        {
            if (_appointments.ContainsKey(appointment.Id))
            {
                throw new InvalidOperationException(
                    $"Já existe um agendamento com o ID {appointment.Id}.");
            }

            _appointments.Add(appointment.Id, appointment);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(appointment);

        lock (_sync)
        {
            if (!_appointments.ContainsKey(appointment.Id))
            {
                throw new KeyNotFoundException(
                    $"Agendamento {appointment.Id} não encontrado.");
            }

            _appointments[appointment.Id] = appointment;
        }

        return Task.CompletedTask;
    }
}
