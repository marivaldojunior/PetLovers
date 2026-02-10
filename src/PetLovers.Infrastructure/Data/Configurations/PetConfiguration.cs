using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetLovers.Domain.Entities;
using PetLovers.Domain.Enums;

namespace PetLovers.Infrastructure.Data.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Species)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Breed)
            .HasMaxLength(100);

        builder.Property(p => p.Age)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.AdoptedAt);

        builder.Property(p => p.AdopterId);

        // Value Object Configuration (Owned Entity)
        builder.OwnsOne(p => p.Characteristics, characteristics =>
        {
            characteristics.Property(c => c.Size)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("CharacteristicsSize");

            characteristics.Property(c => c.Color)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("CharacteristicsColor");

            characteristics.Property(c => c.CoatType)
                .HasMaxLength(50)
                .HasColumnName("CharacteristicsCoatType");

            characteristics.Property(c => c.IsVaccinated)
                .HasColumnName("CharacteristicsIsVaccinated");

            characteristics.Property(c => c.IsNeutered)
                .HasColumnName("CharacteristicsIsNeutered");
        });

        // Indexes
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Species);
        builder.HasIndex(p => new { p.Status, p.Species });
    }
}
