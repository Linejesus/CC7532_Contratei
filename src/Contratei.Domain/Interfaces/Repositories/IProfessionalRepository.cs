using Contratei.Domain.Entities;

namespace Contratei.Domain.Interfaces.Repositories;

public interface IProfessionalRepository
{
    Task<Professional?> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<Professional>> GetAllAsync();

    Task AddAsync(Professional professional);
}