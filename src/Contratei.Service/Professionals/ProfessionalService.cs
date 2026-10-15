using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;
using Contratei.Domain.ValueObjects;

namespace Contratei.Service.Professionals;

public class ProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;

    public ProfessionalService(
        IProfessionalRepository professionalRepository)
    {
        _professionalRepository = professionalRepository;
    }

    public async Task<Guid> CreateAsync(
        CreateProfessionalRequest request)
    {
        var address = new Address(
            request.Street,
            request.Number,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode,
            request.Latitude,
            request.Longitude);

        var professional = new Professional(
            request.Name,
            request.Description,
            request.AreaAtuacao,
            request.Email,
            request.Phone,
            address);

        await _professionalRepository.AddAsync(professional);

        return professional.Id;
    }

    public async Task<IReadOnlyCollection<Professional>> GetAllAsync()
    {
        return await _professionalRepository.GetAllAsync();
    }

    public async Task<Professional?> GetByIdAsync(Guid id)
    {
        return await _professionalRepository.GetByIdAsync(id);
    }

    public async Task<IReadOnlyCollection<ProfessionalSearchResult>>
        SearchAsync(
            double? latitude,
            double? longitude,
            double? radiusKm,
            string? areaAtuacao)
    {
        var hasLatitude = latitude.HasValue;
        var hasLongitude = longitude.HasValue;
        var hasRadius = radiusKm.HasValue;

        var hasAnyLocationFilter =
            hasLatitude || hasLongitude || hasRadius;

        // Se usar localização, os três parâmetros são obrigatórios.
        if (hasAnyLocationFilter &&
            !(hasLatitude && hasLongitude && hasRadius))
        {
            throw new ArgumentException(
                "Informe latitude, longitude e radiusKm juntos.");
        }

        if (!hasAnyLocationFilter &&
            string.IsNullOrWhiteSpace(areaAtuacao))
        {
            throw new ArgumentException(
                "Informe ao menos a área de atuação ou a localização.");
        }

        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitude inválida.");

        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitude inválida.");

        if (radiusKm.HasValue &&
            (!double.IsFinite(radiusKm.Value) || radiusKm.Value <= 0))
        {
            throw new ArgumentException(
                "O raio deve ser um número maior que zero.");
        }

        var professionals = await _professionalRepository.GetAllAsync();

        var results = professionals
            .Where(professional =>
                string.IsNullOrWhiteSpace(areaAtuacao) ||
                string.Equals(
                    professional.AreaAtuacao.Trim(),
                    areaAtuacao.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            .Select(professional =>
            {
                double? distanceKm = null;

                if (latitude.HasValue &&
                    longitude.HasValue &&
                    radiusKm.HasValue)
                {
                    distanceKm = CalculateDistanceKm(
                        latitude.Value,
                        longitude.Value,
                        professional.Address.Latitude,
                        professional.Address.Longitude);
                }

                return new ProfessionalSearchResult(
                    professional,
                    distanceKm);
            })
            .Where(result =>
                !radiusKm.HasValue ||
                result.DistanceKm <= radiusKm.Value)
            .OrderBy(result => result.DistanceKm ?? double.MaxValue)
            .ToList();

        return results;
    }

    private static double CalculateDistanceKm(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double earthRadiusKm = 6371.0;

        var lat1 = degreesToRadians(latitude1);
        var lat2 = degreesToRadians(latitude2);
        var deltaLat = degreesToRadians(latitude2 - latitude1);
        var deltaLon = degreesToRadians(longitude2 - longitude1);

        var a =
            Math.Pow(Math.Sin(deltaLat / 2), 2) +
            Math.Cos(lat1) * Math.Cos(lat2) *
            Math.Pow(Math.Sin(deltaLon / 2), 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;

        static double degreesToRadians(double degrees) =>
            degrees * Math.PI / 180.0;
    }
}