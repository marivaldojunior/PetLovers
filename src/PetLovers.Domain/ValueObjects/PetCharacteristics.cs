namespace PetLovers.Domain.ValueObjects;

public sealed class PetCharacteristics : IEquatable<PetCharacteristics>
{
    public string Size { get; private set; }
    public string Color { get; private set; }
    public string CoatType { get; private set; }
    public bool IsVaccinated { get; private set; }
    public bool IsNeutered { get; private set; }

    private PetCharacteristics() 
    {
        Size = string.Empty;
        Color = string.Empty;
        CoatType = string.Empty;
    }

    public PetCharacteristics(
        string size,
        string color,
        string coatType,
        bool isVaccinated,
        bool isNeutered)
    {
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size cannot be empty.", nameof(size));
        
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color cannot be empty.", nameof(color));

        Size = size;
        Color = color;
        CoatType = coatType ?? string.Empty;
        IsVaccinated = isVaccinated;
        IsNeutered = isNeutered;
    }

    public bool Equals(PetCharacteristics? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Size == other.Size &&
               Color == other.Color &&
               CoatType == other.CoatType &&
               IsVaccinated == other.IsVaccinated &&
               IsNeutered == other.IsNeutered;
    }

    public override bool Equals(object? obj) => Equals(obj as PetCharacteristics);

    public override int GetHashCode() => 
        HashCode.Combine(Size, Color, CoatType, IsVaccinated, IsNeutered);

    public static bool operator ==(PetCharacteristics? left, PetCharacteristics? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(PetCharacteristics? left, PetCharacteristics? right) =>
        !(left == right);
}
