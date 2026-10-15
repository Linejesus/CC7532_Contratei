namespace Contratei.Domain.Entities;

public class ProfessionalAvailability
{
    public Guid Id { get; private set; }

    public Guid ProfessionalId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public string TimeZoneId { get; private set; }

    public bool IsActive { get; private set; }

    public ProfessionalAvailability(
        Guid professionalId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        string timeZoneId = "America/Sao_Paulo")
    {
        if (professionalId == Guid.Empty)
            throw new ArgumentException(
                "O profissional é obrigatório.", nameof(professionalId));

        if (startTime >= endTime)
            throw new ArgumentException(
                "O horário de término deve ser posterior ao início.");

        if (string.IsNullOrWhiteSpace(timeZoneId))
            throw new ArgumentException(
                "O fuso horário é obrigatório.", nameof(timeZoneId));

        Id = Guid.NewGuid();
        ProfessionalId = professionalId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        TimeZoneId = timeZoneId;
        IsActive = true;
    }

    public void UpdateHours(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException(
                "O horário de término deve ser posterior ao início.");

        StartTime = startTime;
        EndTime = endTime;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}