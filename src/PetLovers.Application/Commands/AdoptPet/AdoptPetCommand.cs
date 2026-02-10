using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Commands.AdoptPet;

/// <summary>
/// Command to initiate the adoption process for a pet.
/// </summary>
public sealed record AdoptPetCommand(Guid PetId, Guid AdopterId) : ICommand;
