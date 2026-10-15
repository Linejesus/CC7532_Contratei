namespace Contratei.Service.Availability;

public sealed record ConfigureAvailabilityRequest(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string TimeZoneId = "America/Sao_Paulo"
);