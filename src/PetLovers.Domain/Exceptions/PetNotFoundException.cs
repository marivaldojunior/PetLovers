namespace PetLovers.Domain.Exceptions;

public class PetNotFoundException : DomainException
{
    public Guid PetId { get; }

    public PetNotFoundException(Guid petId) 
        : base($"Pet with ID '{petId}' was not found.")
    {
        PetId = petId;
    }
}
