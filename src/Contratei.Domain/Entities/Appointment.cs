namespace Contratei.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }

    public Guid ProfessionalId { get; private set; }

    public Guid ContractorId { get; private set; }

    public Guid ProfessionalServiceId { get; private set; }

    public DateTimeOffset StartAt { get; private set; }

    public DateTimeOffset EndAt { get; private set; }

    public decimal PriceAtBooking { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public string? GoogleCalendarEventId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public Appointment(
        Guid professionalId,
        Guid contractorId,
        Guid professionalServiceId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        decimal priceAtBooking)
    {
        if (professionalId == Guid.Empty)
            throw new ArgumentException(
                "O profissional é obrigatório.", nameof(professionalId));

        if (contractorId == Guid.Empty)
            throw new ArgumentException(
                "O contratante é obrigatório.", nameof(contractorId));

        if (professionalServiceId == Guid.Empty)
            throw new ArgumentException(
                "O serviço é obrigatório.", nameof(professionalServiceId));

        if (endAt <= startAt)
            throw new ArgumentException(
                "O horário de término deve ser posterior ao início.");

        if (priceAtBooking < 0)
            throw new ArgumentOutOfRangeException(
                nameof(priceAtBooking),
                "O preço não pode ser negativo.");

        Id = Guid.NewGuid();
        ProfessionalId = professionalId;
        ContractorId = contractorId;
        ProfessionalServiceId = professionalServiceId;
        StartAt = startAt;
        EndAt = endAt;
        PriceAtBooking = priceAtBooking;
        Status = AppointmentStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException(
                "Somente agendamentos pendentes podem ser confirmados.");

        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status is AppointmentStatus.Cancelled
            or AppointmentStatus.Completed)
        {
            throw new InvalidOperationException(
                "Este agendamento não pode ser cancelado.");
        }

        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
    }

    public void SetGoogleCalendarEventId(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new ArgumentException(
                "O identificador do evento é obrigatório.", nameof(eventId));

        GoogleCalendarEventId = eventId;
    }
}
