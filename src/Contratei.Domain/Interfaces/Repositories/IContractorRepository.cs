using Contratei.Domain.Entities;

namespace Contratei.Domain.Interfaces.Repositories;

public interface IContractorRepository
{
    Task<Contractor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Contractor?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Contractor>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Contractor contractor,
        CancellationToken cancellationToken = default);
}