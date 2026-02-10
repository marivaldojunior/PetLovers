using PetLovers.Domain.Enums;

namespace PetLovers.Application.DTOs;

public record PetDto(
    Guid Id,
    string Name,
    string Species,
    string Breed,
    int Age,
    string Description,
    string Status,
    PetCharacteristicsDto Characteristics,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? AdoptedAt);

public record PetCharacteristicsDto(
    string Size,
    string Color,
    string CoatType,
    bool IsVaccinated,
    bool IsNeutered);

public record CreatePetDto(
    string Name,
    PetSpecies Species,
    string Breed,
    int Age,
    string Description,
    PetCharacteristicsDto Characteristics);

public record UpdatePetDto(
    string Name,
    string Breed,
    int Age,
    string Description,
    PetCharacteristicsDto Characteristics);
