using PetLovers.Application.Interfaces;
using PetLovers.Domain.Exceptions;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Commands.AdoptPet;

public sealed class AdoptPetCommandHandler : ICommandHandler<AdoptPetCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdoptPetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<CommandResult> Handle(AdoptPetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(request.PetId, cancellationToken);
            
            if (pet is null)
                throw new PetNotFoundException(request.PetId);

            pet.MarkAsPending(request.AdopterId);
            
            await _unitOfWork.Pets.UpdateAsync(pet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return CommandResult.Success();
        }
        catch (DomainException ex)
        {
            return CommandResult.Failure(ex.Message);
        }
    }
}
