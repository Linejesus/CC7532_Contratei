namespace Contratei.Service.Appointments;

public sealed record AvailableTimeSlot(
    DateTimeOffset StartAt,
    DateTimeOffset EndAt
);