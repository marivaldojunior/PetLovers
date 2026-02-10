using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Enums;

namespace PetLovers.Application.Commands.RegisterPet;

/// <summary>
/// Command to register a new pet in the adoption system.
/// </summary>
public sealed record RegisterPetCommand(
    string Name,
    PetSpecies Species,
    string Breed,
    int Age,
    string Description,
    PetCharacteristicsDto Characteristics) : ICommand<PetDto>;
