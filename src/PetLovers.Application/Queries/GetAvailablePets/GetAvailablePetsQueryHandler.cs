using AutoMapper;
using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Queries.GetAvailablePets;

public sealed class GetAvailablePetsQueryHandler : IQueryHandler<GetAvailablePetsQuery, IEnumerable<PetDto>>
{
    private readonly IPetRepository _petRepository;
    private readonly IMapper _mapper;

    public GetAvailablePetsQueryHandler(IPetRepository petRepository, IMapper mapper)
    {
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<PetDto>> Handle(
        GetAvailablePetsQuery request, 
        CancellationToken cancellationToken)
    {
        var pets = await _petRepository.GetAvailablePetsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PetDto>>(pets);
    }
}
