using AutoMapper;
using PetLovers.Application.DTOs;
using PetLovers.Domain.Entities;
using PetLovers.Domain.ValueObjects;

namespace PetLovers.Application.Mappings;

public class PetMappingProfile : Profile
{
    public PetMappingProfile()
    {
        CreateMap<Pet, PetDto>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.Species.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<PetCharacteristics, PetCharacteristicsDto>();

        CreateMap<PetCharacteristicsDto, PetCharacteristics>()
            .ConstructUsing(src => new PetCharacteristics(
                src.Size,
                src.Color,
                src.CoatType,
                src.IsVaccinated,
                src.IsNeutered));
    }
}
