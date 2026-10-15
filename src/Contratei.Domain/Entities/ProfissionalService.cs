namespace Contratei.Domain.Entities;

public class ProfessionalService
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public TimeSpan Duration { get; private set; }

    public bool IsActive { get; private set; }

    internal ProfessionalService(
        string name,
        string description,
        decimal price,
        TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "O nome do serviço é obrigatório.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "A descrição do serviço é obrigatória.",
                nameof(description));

        if (price < 0)
            throw new ArgumentException(
                "O preço não pode ser negativo.",
                nameof(price));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException(
                "A duração deve ser maior que zero.",
                nameof(duration));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        IsActive = true;
    }
}