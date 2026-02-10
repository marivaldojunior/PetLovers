using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetLovers.Application.Commands.AdoptPet;
using PetLovers.Application.Commands.ConfirmAdoption;
using PetLovers.Application.Commands.RegisterPet;
using PetLovers.Application.DTOs;
using PetLovers.Application.Queries.GetAvailablePets;
using PetLovers.Application.Queries.GetPetById;
using PetLovers.Application.Queries.GetPetsBySpecies;
using PetLovers.Domain.Enums;

namespace PetLovers.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PetsController> _logger;

    public PetsController(IMediator mediator, ILogger<PetsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all pets available for adoption.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetAvailablePets(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving available pets");
        var result = await _mediator.Send(new GetAvailablePetsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific pet by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetDto>> GetById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving pet with ID: {PetId}", id);
        var result = await _mediator.Send(new GetPetByIdQuery(id), cancellationToken);
        
        if (result is null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get pets by species.
    /// </summary>
    [HttpGet("species/{species}")]
    [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetBySpecies(
        PetSpecies species, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving pets of species: {Species}", species);
        var result = await _mediator.Send(new GetPetsBySpeciesQuery(species), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Register a new pet for adoption.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PetDto>> Register(
        [FromBody] RegisterPetCommand command, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering new pet: {PetName}", command.Name);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
                return BadRequest(new { Errors = result.ValidationErrors });
            
            return BadRequest(new { Error = result.Error });
        }
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Initiate adoption process for a pet.
    /// </summary>
    [HttpPost("{id:guid}/adopt")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Adopt(
        Guid id, 
        [FromBody] AdoptPetRequest request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating adoption for pet: {PetId} by adopter: {AdopterId}", id, request.AdopterId);
        var result = await _mediator.Send(new AdoptPetCommand(id, request.AdopterId), cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });
            
        return NoContent();
    }

    /// <summary>
    /// Confirm the adoption of a pet.
    /// </summary>
    [HttpPost("{id:guid}/confirm-adoption")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmAdoption(
        Guid id, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Confirming adoption for pet: {PetId}", id);
        var result = await _mediator.Send(new ConfirmAdoptionCommand(id), cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });
            
        return NoContent();
    }
}

public record AdoptPetRequest(Guid AdopterId);
