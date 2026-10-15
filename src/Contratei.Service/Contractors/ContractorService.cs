using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Service.Contractors;

public class ContractorService
{
    private readonly IContractorRepository _repository;

    public ContractorService(IContractorRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateAsync(
        CreateContractorRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.");

        var existing = await _repository.GetByEmailAsync(
            email, cancellationToken);

        if (existing is not null)
            throw new InvalidOperationException(
                "Já existe um contratante cadastrado com este e-mail.");

        var contractor = new Contractor(
            request.Name,
            email,
            request.Phone);

        await _repository.AddAsync(contractor, cancellationToken);

        return contractor.Id;
    }

    public Task<Contractor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<Contractor>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }
}