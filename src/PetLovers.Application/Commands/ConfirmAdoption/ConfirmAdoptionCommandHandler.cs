using PetLovers.Application.Interfaces;
using PetLovers.Domain.Exceptions;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Commands.ConfirmAdoption;

public sealed class ConfirmAdoptionCommandHandler : ICommandHandler<ConfirmAdoptionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmAdoptionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<CommandResult> Handle(ConfirmAdoptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(request.PetId, cancellationToken);
            
            if (pet is null)
                throw new PetNotFoundException(request.PetId);

            pet.ConfirmAdoption();
            
            await _unitOfWork.Pets.UpdateAsync(pet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Here we could publish a domain event for the Notification Pattern
            // await _mediator.Publish(new PetAdoptedNotification(pet.Id, pet.AdopterId!.Value), cancellationToken);

            return CommandResult.Success();
        }
        catch (DomainException ex)
        {
            return CommandResult.Failure(ex.Message);
        }
    }
}
