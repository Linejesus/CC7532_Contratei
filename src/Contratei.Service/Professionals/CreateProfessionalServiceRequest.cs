namespace Contratei.Service.Professionals;

public sealed record CreateProfessionalServiceRequest(
    string Name,
    string Description,
    decimal Price,
    int DurationMinutes
);