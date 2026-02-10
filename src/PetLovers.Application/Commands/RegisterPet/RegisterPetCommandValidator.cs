using FluentValidation;

namespace PetLovers.Application.Commands.RegisterPet;

public sealed class RegisterPetCommandValidator : AbstractValidator<RegisterPetCommand>
{
    public RegisterPetCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pet name is required.")
            .MaximumLength(100).WithMessage("Pet name cannot exceed 100 characters.");

        RuleFor(x => x.Species)
            .IsInEnum().WithMessage("Invalid pet species.");

        RuleFor(x => x.Breed)
            .MaximumLength(100).WithMessage("Breed cannot exceed 100 characters.");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(0).WithMessage("Age cannot be negative.")
            .LessThanOrEqualTo(50).WithMessage("Age seems unrealistic. Please verify.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Characteristics)
            .NotNull().WithMessage("Characteristics are required.")
            .SetValidator(new PetCharacteristicsValidator());
    }
}

internal class PetCharacteristicsValidator : AbstractValidator<DTOs.PetCharacteristicsDto>
{
    public PetCharacteristicsValidator()
    {
        RuleFor(x => x.Size)
            .NotEmpty().WithMessage("Size is required.");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required.");
    }
}
