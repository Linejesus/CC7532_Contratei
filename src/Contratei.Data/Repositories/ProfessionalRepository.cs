using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Data.Repositories;

public class ProfessionalRepository : IProfessionalRepository
{
    private readonly List<Professional> _professionals = new();

    public Task<Professional?> GetByIdAsync(Guid id)
    {
        var professional = _professionals
            .FirstOrDefault(p => p.Id == id);

        return Task.FromResult(professional);
    }

    public Task<IReadOnlyCollection<Professional>> GetAllAsync()
    {
        IReadOnlyCollection<Professional> professionals =
            _professionals.ToList();

        return Task.FromResult(professionals);
    }

    public Task AddAsync(Professional professional)
    {
        _professionals.Add(professional);

        return Task.CompletedTask;
    }
}
