using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Commands.ConfirmAdoption;

public sealed record ConfirmAdoptionCommand(Guid PetId) : ICommand;
