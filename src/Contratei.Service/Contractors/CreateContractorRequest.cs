namespace Contratei.Service.Contractors;

public sealed record CreateContractorRequest(
    string Name,
    string Email,
    string Phone
);