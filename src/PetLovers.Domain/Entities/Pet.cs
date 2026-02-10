using PetLovers.Domain.Enums;
using PetLovers.Domain.Exceptions;
using PetLovers.Domain.ValueObjects;

namespace PetLovers.Domain.Entities;

/// <summary>
/// Represents a pet available for adoption.
/// Implements Rich Domain Model with encapsulated state transitions.
/// </summary>
public class Pet
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public PetSpecies Species { get; private set; }
    public string Breed { get; private set; }
    public int Age { get; private set; }
    public string Description { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public PetCharacteristics Characteristics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? AdoptedAt { get; private set; }
    public Guid? AdopterId { get; private set; }

    // EF Core constructor
    private Pet()
    {
        Name = string.Empty;
        Breed = string.Empty;
        Description = string.Empty;
        Characteristics = null!;
    }

    /// <summary>
    /// Creates a new Pet with initial Available status.
    /// </summary>
    public Pet(
        string name,
        PetSpecies species,
        string breed,
        int age,
        string description,
        PetCharacteristics characteristics)
    {
        ValidateName(name);
        ValidateAge(age);
        ValidateDescription(description);

        Id = Guid.NewGuid();
        Name = name;
        Species = species;
        Breed = breed ?? string.Empty;
        Age = age;
        Description = description;
        Characteristics = characteristics ?? throw new ArgumentNullException(nameof(characteristics));
        Status = AdoptionStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates basic pet information. Cannot be called when pet is Adopted.
    /// </summary>
    public void UpdateInfo(
        string name,
        string breed,
        int age,
        string description,
        PetCharacteristics characteristics)
    {
        if (Status == AdoptionStatus.Adopted)
            throw new InvalidPetStateException(Status.ToString(), "UpdateInfo");

        ValidateName(name);
        ValidateAge(age);
        ValidateDescription(description);

        Name = name;
        Breed = breed ?? string.Empty;
        Age = age;
        Description = description;
        Characteristics = characteristics ?? throw new ArgumentNullException(nameof(characteristics));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the pet as pending adoption.
    /// Only Available pets can transition to Pending.
    /// </summary>
    public void MarkAsPending(Guid adopterId)
    {
        if (Status != AdoptionStatus.Available)
            throw new InvalidPetStateException(Status.ToString(), "MarkAsPending");

        if (adopterId == Guid.Empty)
            throw new ArgumentException("Adopter ID cannot be empty.", nameof(adopterId));

        Status = AdoptionStatus.Pending;
        AdopterId = adopterId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirms the adoption of the pet.
    /// Only Pending pets can be confirmed as Adopted.
    /// </summary>
    public void ConfirmAdoption()
    {
        if (Status != AdoptionStatus.Pending)
            throw new InvalidPetStateException(Status.ToString(), "ConfirmAdoption");

        Status = AdoptionStatus.Adopted;
        AdoptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the pending adoption and makes the pet available again.
    /// Only Pending pets can have their adoption cancelled.
    /// </summary>
    public void CancelAdoption()
    {
        if (Status != AdoptionStatus.Pending)
            throw new InvalidPetStateException(Status.ToString(), "CancelAdoption");

        Status = AdoptionStatus.Available;
        AdopterId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Returns the pet to available status (e.g., if returned by adopter).
    /// Only Adopted pets can be returned.
    /// </summary>
    public void ReturnToShelter()
    {
        if (Status != AdoptionStatus.Adopted)
            throw new InvalidPetStateException(Status.ToString(), "ReturnToShelter");

        Status = AdoptionStatus.Available;
        AdopterId = null;
        AdoptedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailableForAdoption() => Status == AdoptionStatus.Available;

    #region Validation Methods

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Pet name cannot be empty.");

        if (name.Length > 100)
            throw new DomainException("Pet name cannot exceed 100 characters.");
    }

    private static void ValidateAge(int age)
    {
        if (age < 0)
            throw new DomainException("Pet age cannot be negative.");

        if (age > 50)
            throw new DomainException("Pet age seems unrealistic. Please verify.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Pet description cannot be empty.");

        if (description.Length > 2000)
            throw new DomainException("Pet description cannot exceed 2000 characters.");
    }

    #endregion
}
