using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Enums;

namespace PetLovers.Application.Queries.GetPetsBySpecies;

/// <summary>
/// Query to retrieve pets by species.
/// </summary>
public sealed record GetPetsBySpeciesQuery(PetSpecies Species) : IQuery<IEnumerable<PetDto>>;
