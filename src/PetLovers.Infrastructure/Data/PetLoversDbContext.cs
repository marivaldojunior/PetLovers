using Microsoft.EntityFrameworkCore;
using PetLovers.Domain.Entities;
using System.Reflection;

namespace PetLovers.Infrastructure.Data;

public class PetLoversDbContext : DbContext
{
    public PetLoversDbContext(DbContextOptions<PetLoversDbContext> options) : base(options)
    {
    }

    public DbSet<Pet> Pets => Set<Pet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
