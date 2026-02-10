using FluentValidation;

namespace PetLovers.Application.Commands.AdoptPet;

public sealed class AdoptPetCommandValidator : AbstractValidator<AdoptPetCommand>
{
    public AdoptPetCommandValidator()
    {
        RuleFor(x => x.PetId)
            .NotEmpty().WithMessage("Pet ID is required.");

        RuleFor(x => x.AdopterId)
            .NotEmpty().WithMessage("Adopter ID is required.");
    }
}
