using AutoMapper;
using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Queries.GetPetById;

public sealed class GetPetByIdQueryHandler : IQueryHandler<GetPetByIdQuery, PetDto?>
{
    private readonly IPetRepository _petRepository;
    private readonly IMapper _mapper;

    public GetPetByIdQueryHandler(IPetRepository petRepository, IMapper mapper)
    {
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PetDto?> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdAsync(request.Id, cancellationToken);
        return pet is null ? null : _mapper.Map<PetDto>(pet);
    }
}
