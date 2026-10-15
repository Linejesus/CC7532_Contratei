
using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;
using ProfessionalServiceEntity =
    Contratei.Domain.Entities.ProfessionalService;

namespace Contratei.Service.Professionals;

public sealed class ProfessionalCatalogService
{
    private readonly IProfessionalRepository _professionalRepository;

    public ProfessionalCatalogService(
        IProfessionalRepository professionalRepository)
    {
        _professionalRepository = professionalRepository;
    }

    public async Task<ProfessionalServiceEntity> CreateAsync(
        Guid professionalId,
        CreateProfessionalServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.DurationMinutes <= 0)
        {
            throw new ArgumentException(
                "A duração deve ser maior que zero.");
        }

        var professional =
            await _professionalRepository.GetByIdAsync(professionalId);

        if (professional is null)
        {
            throw new KeyNotFoundException(
                "Profissional não encontrado.");
        }

        return professional.AddService(
            request.Name,
            request.Description,
            request.Price,
            TimeSpan.FromMinutes(request.DurationMinutes));
    }

    public async Task<IReadOnlyCollection<ProfessionalServiceEntity>>
        GetByProfessionalIdAsync(
            Guid professionalId,
            CancellationToken cancellationToken = default)
    {
        var professional =
            await _professionalRepository.GetByIdAsync(professionalId);

        if (professional is null)
        {
            throw new KeyNotFoundException(
                "Profissional não encontrado.");
        }

        return professional.Services.ToList();
    }
}
