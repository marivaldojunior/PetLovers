using PetLovers.Domain.Entities;
using PetLovers.Domain.Enums;

namespace PetLovers.Domain.Interfaces;

public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetByStatusAsync(AdoptionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetBySpeciesAsync(PetSpecies species, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetAvailablePetsAsync(CancellationToken cancellationToken = default);
    Task<Pet> AddAsync(Pet pet, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(AdoptionStatus status, CancellationToken cancellationToken = default);
}
