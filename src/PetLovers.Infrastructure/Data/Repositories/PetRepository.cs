using Microsoft.EntityFrameworkCore;
using PetLovers.Domain.Entities;
using PetLovers.Domain.Enums;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Infrastructure.Data.Repositories;

public class PetRepository : IPetRepository
{
    private readonly PetLoversDbContext _context;

    public PetRepository(PetLoversDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetByStatusAsync(AdoptionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .AsNoTracking()
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetBySpeciesAsync(PetSpecies species, CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .AsNoTracking()
            .Where(p => p.Species == species)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetAvailablePetsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .AsNoTracking()
            .Where(p => p.Status == AdoptionStatus.Available)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pet> AddAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        await _context.Pets.AddAsync(pet, cancellationToken);
        return pet;
    }

    public Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        _context.Pets.Update(pet);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pet = await GetByIdAsync(id, cancellationToken);
        if (pet is not null)
        {
            _context.Pets.Remove(pet);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pets.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountByStatusAsync(AdoptionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Pets.CountAsync(p => p.Status == status, cancellationToken);
    }
}
