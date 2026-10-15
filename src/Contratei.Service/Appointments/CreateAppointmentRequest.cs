
namespace Contratei.Service.Appointments;

public sealed record CreateAppointmentRequest(
    Guid ProfessionalId,
    Guid ContractorId,
    Guid ProfessionalServiceId,
    DateTimeOffset StartAt
);
