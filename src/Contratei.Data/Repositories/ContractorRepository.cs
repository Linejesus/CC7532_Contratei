using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Data.Repositories;

public class ContractorRepository : IContractorRepository
{
    private readonly Dictionary<Guid, Contractor> _contractors = new();
    private readonly object _lock = new();

    public Task<Contractor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _contractors.TryGetValue(id, out var contractor);
            return Task.FromResult(contractor);
        }
    }

    public Task<Contractor?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            var contractor = _contractors.Values.FirstOrDefault(
                c => string.Equals(
                    c.Email,
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(contractor);
        }
    }

    public Task<IReadOnlyList<Contractor>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            IReadOnlyList<Contractor> result =
                _contractors.Values.ToList();

            return Task.FromResult(result);
        }
    }

    public Task AddAsync(
        Contractor contractor,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            if (_contractors.Values.Any(c =>
                string.Equals(
                    c.Email,
                    contractor.Email,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Já existe um contratante cadastrado com este e-mail.");
            }

            _contractors.Add(contractor.Id, contractor);
        }

        return Task.CompletedTask;
    }
}