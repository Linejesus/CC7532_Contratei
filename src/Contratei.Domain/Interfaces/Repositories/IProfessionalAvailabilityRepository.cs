
using Contratei.Domain.Entities;

namespace Contratei.Domain.Interfaces.Repositories;

public interface IProfessionalAvailabilityRepository
{
    Task<IReadOnlyList<ProfessionalAvailability>> GetByProfessionalIdAsync(
        Guid professionalId,
        CancellationToken cancellationToken = default);

    Task<ProfessionalAvailability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProfessionalAvailability availability,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProfessionalAvailability availability,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
