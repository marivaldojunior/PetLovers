using AutoMapper;
using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Queries.GetPetsBySpecies;

public sealed class GetPetsBySpeciesQueryHandler : IQueryHandler<GetPetsBySpeciesQuery, IEnumerable<PetDto>>
{
    private readonly IPetRepository _petRepository;
    private readonly IMapper _mapper;

    public GetPetsBySpeciesQueryHandler(IPetRepository petRepository, IMapper mapper)
    {
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<PetDto>> Handle(
        GetPetsBySpeciesQuery request, 
        CancellationToken cancellationToken)
    {
        var pets = await _petRepository.GetBySpeciesAsync(request.Species, cancellationToken);
        return _mapper.Map<IEnumerable<PetDto>>(pets);
    }
}
