namespace Contratei.Domain.Entities;

public class Contractor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Contractor(string name, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome é obrigatório.", nameof(name));

        if (string.IsNullOrWhiteSpace(email) ||
            !System.Net.Mail.MailAddress.TryCreate(email, out var address))
            throw new ArgumentException("Informe um e-mail válido.", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("O telefone é obrigatório.", nameof(phone));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = address.Address.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
    }
}