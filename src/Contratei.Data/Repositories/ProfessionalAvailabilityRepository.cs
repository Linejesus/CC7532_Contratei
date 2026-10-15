using Contratei.Domain.Entities;
using Contratei.Domain.Interfaces.Repositories;

namespace Contratei.Data.Repositories;

public class ProfessionalAvailabilityRepository
    : IProfessionalAvailabilityRepository
{
    private readonly Dictionary<Guid, ProfessionalAvailability>
        _availabilities = new();

    private readonly object _sync = new();

    public Task<IReadOnlyList<ProfessionalAvailability>>
        GetByProfessionalIdAsync(
            Guid professionalId,
            CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (professionalId == Guid.Empty)
        {
            throw new ArgumentException(
                "O profissional é obrigatório.",
                nameof(professionalId));
        }

        lock (_sync)
        {
            var availabilities = _availabilities.Values
                .Where(a => a.ProfessionalId == professionalId)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToList();

            return Task.FromResult<
                IReadOnlyList<ProfessionalAvailability>>(
                    availabilities);
        }
    }

    public Task<ProfessionalAvailability?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            _availabilities.TryGetValue(id, out var availability);
            return Task.FromResult(availability);
        }
    }

    public Task AddAsync(
        ProfessionalAvailability availability,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(availability);

        lock (_sync)
        {
            if (_availabilities.ContainsKey(availability.Id))
            {
                throw new InvalidOperationException(
                    $"Já existe uma disponibilidade com o ID " +
                    $"{availability.Id}.");
            }

            _availabilities.Add(availability.Id, availability);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        ProfessionalAvailability availability,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(availability);

        lock (_sync)
        {
            if (!_availabilities.ContainsKey(availability.Id))
            {
                throw new KeyNotFoundException(
                    $"Disponibilidade {availability.Id} não encontrada.");
            }

            _availabilities[availability.Id] = availability;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (!_availabilities.Remove(id))
            {
                throw new KeyNotFoundException(
                    $"Disponibilidade {id} não encontrada.");
            }
        }

        return Task.CompletedTask;
    }
}
