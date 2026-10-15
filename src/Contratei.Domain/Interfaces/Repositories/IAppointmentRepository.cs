
using Contratei.Domain.Entities;

namespace Contratei.Domain.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByProfessionalAndPeriodAsync(
        Guid professionalId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByContractorIdAsync(
        Guid contractorId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByProfessionalIdAsync(
        Guid professionalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default);
}
