using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Extensions;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.PositionModels;
using OrganizationalStructure.Validators;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly PositionValidator _validator;

    public PositionsController(
        IPositionRepository positionRepository, 
        IUnitOfWork unitOfWork,
        PositionValidator validator)
    {
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;

        _validator = validator;
    }

    // GET: api/Positions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PositionModel>>> GetPositions()
    {
        if (_positionRepository == null) return NotFound();

        return (await _positionRepository.GetAllAsync())
            .Select(x => new PositionModel(x)).ToList();
    }

    // GET: api/Positions/{Guid}
    [HttpGet("{id}")]
    public async Task<ActionResult<PositionModel>> GetPosition(Guid id)
    {
        if (_positionRepository == null) return NotFound();

        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotFound();

        return new PositionModel(position);
    }

    // PUT: api/Positions/{Guid}
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPosition(Guid id, Position position)
    {
        if (id != position.Id) return BadRequest();
        if (_positionRepository == null || _unitOfWork == null) return NotFound();

        var results = _validator.Validate(position);
        if (!results.IsValid)
            return ValidationProblem(results.Errors.GetStringErrorMessange());

        var pos = await _positionRepository.GetByIdAsync(position.Id);
        if (pos == null) return NotFound();

        pos.Name = !string.IsNullOrEmpty(position.Name) ? position.Name : pos.Name;

        try
        {
            await _unitOfWork.Commit();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _positionRepository.IsExists(x => x.Id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // POST: api/Positions
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<PositionModel>> PostPosition(Position position)
    {
        if (_positionRepository == null || _unitOfWork == null) return NotFound();

        var results = _validator.Validate(position);
        if (!results.IsValid)
            return ValidationProblem(results.Errors.GetStringErrorMessange());

        var newPosition = new Position { Id = Guid.NewGuid() };

        try
        {
            if (string.IsNullOrEmpty(position.Name)) return BadRequest("Не указано название професси");
            newPosition.Name = position.Name;

            newPosition = await _positionRepository.AddAsync(newPosition);
            await _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
       
        return CreatedAtAction("GetPosition", new { id = newPosition.Id }, new PositionModel(newPosition));
    }

    // DELETE: api/Positions/{Guid}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePosition(Guid id)
    {
        if (_positionRepository == null || _unitOfWork == null) return NotFound();

        var position = new Position { Id = id };

        try
        {
            _positionRepository.Delete(position);
            await _unitOfWork.Commit();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return NotFound($"Record does not exist in the database. Message: {e.Message}");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return NoContent();
    }
}
