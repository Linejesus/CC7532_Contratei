using Contratei.Domain.ValueObjects;

namespace Contratei.Domain.Entities;

public class Professional
{
    private readonly List<ProfessionalService> _services = new();

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string AreaAtuacao { get; private set; }

    public string Email { get; private set; }

    public string Phone { get; private set; }

    public Address Address { get; private set; }

    public IReadOnlyCollection<ProfessionalService> Services =>
        _services.AsReadOnly();

    public Professional(
        string name,
        string description,
        string areaAtuacao,
        string email,
        string phone,
        Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "O nome do profissional é obrigatório.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "A descrição do profissional é obrigatória.",
                nameof(description));

        if (string.IsNullOrWhiteSpace(areaAtuacao))
            throw new ArgumentException(
                "A área de atuação do profissional é obrigatória.",
                nameof(areaAtuacao));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "O e-mail do profissional é obrigatório.",
                nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException(
                "O telefone do profissional é obrigatório.",
                nameof(phone));

        ArgumentNullException.ThrowIfNull(address);

        Id = Guid.NewGuid();

        Name = name;
        Description = description;
        AreaAtuacao = areaAtuacao;
        Email = email;
        Phone = phone;
        Address = address;
    }

    
    public ProfessionalService AddService(
        string name,
        string description,
        decimal price,
        TimeSpan duration)
    {
        var service = new ProfessionalService(
            name,
            description,
            price,
            duration);

        _services.Add(service);

        return service;
    }

}