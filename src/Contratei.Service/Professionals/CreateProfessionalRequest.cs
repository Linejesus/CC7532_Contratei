namespace Contratei.Service.Professionals;

public record CreateProfessionalRequest(
    string Name,
    string Description,
    string AreaAtuacao,
    string Email,
    string Phone,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    double Latitude,
    double Longitude);