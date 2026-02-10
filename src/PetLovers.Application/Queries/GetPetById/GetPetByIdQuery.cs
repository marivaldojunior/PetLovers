using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Queries.GetPetById;

/// <summary>
/// Query to retrieve a specific pet by its ID.
/// </summary>
public sealed record GetPetByIdQuery(Guid Id) : IQuery<PetDto?>;
