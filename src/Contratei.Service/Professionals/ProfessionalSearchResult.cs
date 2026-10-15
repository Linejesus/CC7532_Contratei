using Contratei.Domain.Entities;

namespace Contratei.Service.Professionals;

public sealed record ProfessionalSearchResult(
    Professional Professional,
    double? DistanceKm);