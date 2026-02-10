namespace PetLovers.Domain.Exceptions;

public class InvalidPetStateException : DomainException
{
    public InvalidPetStateException(string message) : base(message)
    {
    }

    public InvalidPetStateException(string currentState, string attemptedAction)
        : base($"Cannot perform '{attemptedAction}' when pet status is '{currentState}'.")
    {
    }
}
