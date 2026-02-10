using AutoMapper;
using PetLovers.Application.DTOs;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Entities;
using PetLovers.Domain.Interfaces;
using PetLovers.Domain.ValueObjects;

namespace PetLovers.Application.Commands.RegisterPet;

/// <summary>
/// Handler for RegisterPetCommand. Creates a new pet and persists it.
/// </summary>
public sealed class RegisterPetCommandHandler : ICommandHandler<RegisterPetCommand, PetDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterPetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CommandResult<PetDto>> Handle(
        RegisterPetCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var characteristics = new PetCharacteristics(
                request.Characteristics.Size,
                request.Characteristics.Color,
                request.Characteristics.CoatType,
                request.Characteristics.IsVaccinated,
                request.Characteristics.IsNeutered);

            var pet = new Pet(
                request.Name,
                request.Species,
                request.Breed,
                request.Age,
                request.Description,
                characteristics);

            await _unitOfWork.Pets.AddAsync(pet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var petDto = _mapper.Map<PetDto>(pet);
            return CommandResult<PetDto>.Success(petDto);
        }
        catch (Exception ex)
        {
            return CommandResult<PetDto>.Failure(ex.Message);
        }
    }
}
