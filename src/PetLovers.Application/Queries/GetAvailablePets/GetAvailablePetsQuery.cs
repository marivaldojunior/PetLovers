using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Queries.GetAvailablePets;

/// <summary>
/// Query to retrieve all pets available for adoption.
/// </summary>
public sealed record GetAvailablePetsQuery : IQuery<IEnumerable<PetDto>>;
