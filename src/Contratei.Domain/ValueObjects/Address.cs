namespace Contratei.Domain.ValueObjects;

public class Address
{
    public string Street { get; private set; }

    public string Number { get; private set; }

    public string Neighborhood { get; private set; }

    public string City { get; private set; }

    public string State { get; private set; }

    public string ZipCode { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public Address(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException(
                "A rua é obrigatória.",
                nameof(street));

        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException(
                "O número é obrigatório.",
                nameof(number));

        if (string.IsNullOrWhiteSpace(neighborhood))
            throw new ArgumentException(
                "O bairro é obrigatório.",
                nameof(neighborhood));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException(
                "A cidade é obrigatória.",
                nameof(city));

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException(
                "O estado é obrigatório.",
                nameof(state));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException(
                "O CEP é obrigatório.",
                nameof(zipCode));

        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Latitude = latitude;
        Longitude = longitude;
    }
}