using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Extensions;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.OrgStructureModels;
using OrganizationalStructure.Validators;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrgStructuresController : ControllerBase
{
    private readonly IOrgStructureRepository _orgStructureRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly OrgStructureValidator _validator;

    public OrgStructuresController(
        IOrgStructureRepository orgStructureRepository,
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        OrgStructureValidator validator)
    {
        _orgStructureRepository = orgStructureRepository;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;

        _validator = validator;
    }

    // GET: api/OrgStructures
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrgStructureModel>>> GetOrgStructures()
    {
        if (_orgStructureRepository == null) return NotFound();

        return (await _orgStructureRepository.GetAllAsync())
            .Select(structure => new OrgStructureModel(structure)).ToList();
    }

    // GET: api/OrgStructures/{Guid}
    [HttpGet("{id}")]
    public async Task<ActionResult<OrgStructureModel>> GetOrgStructure(Guid id)
    {
        if (_orgStructureRepository == null) return NotFound();

        var orgStructure = await _orgStructureRepository.GetByIdAsync(id);
        if (orgStructure == null) return NotFound();

        return new OrgStructureModel(orgStructure);
    }

    // PUT: api/OrgStructures/{Guid}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrgStructure(Guid id, OrgStructure orgStructure)
    {
        if (id != orgStructure.Id) return BadRequest();
        if (_orgStructureRepository == null ||
            _departmentRepository == null ||
            _positionRepository == null ||
            _userRepository == null ||
            _unitOfWork == null)
            return NotFound();

        var results = _validator.Validate(orgStructure);
        if (!results.IsValid)
            return ValidationProblem(results.Errors.GetStringErrorMessange());

        try
        {
            var structure = await _orgStructureRepository.GetByIdAsync(orgStructure.Id);
            if (structure == null) return NotFound();

            structure.Position = await _positionRepository.GetByIdAsync(orgStructure.PositionId) ?? structure.Position;
            structure.Department = await _departmentRepository.GetByIdAsync(orgStructure.DepartmentId) ?? structure.Department;
            structure.User = await _userRepository.GetByIdAsync(orgStructure.UserId) ?? structure.User;

            await _unitOfWork.Commit();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _orgStructureRepository.IsExists(x => x.Id == id)) return NotFound();
            else throw;
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    // POST: api/OrgStructures
    [HttpPost]
    public async Task<ActionResult<OrgStructureModel>> PostOrgStructure(OrgStructure orgStructure)
    {
        if (_orgStructureRepository == null ||
            _departmentRepository == null ||
            _positionRepository == null ||
            _userRepository == null ||
            _unitOfWork == null)
            return NotFound();

        var results = _validator.Validate(orgStructure);
        if (!results.IsValid)
            return ValidationProblem(results.Errors.GetStringErrorMessange());

        var newOrgStructure = new OrgStructure { Id = Guid.NewGuid() };

        try
        {
            newOrgStructure.Position = await _positionRepository.GetByIdAsync(orgStructure.PositionId);
            newOrgStructure.Department = await _departmentRepository.GetByIdAsync(orgStructure.DepartmentId);
            newOrgStructure.User = await _userRepository.GetByIdAsync(orgStructure.UserId);

            newOrgStructure = await _orgStructureRepository.AddAsync(newOrgStructure);
            await _unitOfWork.Commit();

            return CreatedAtAction(nameof(GetOrgStructure), new { id = newOrgStructure.Id }, new OrgStructureModel(newOrgStructure));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    // DELETE: api/OrgStructures/{Guid}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrgStructure(Guid id)
    {
        if (_orgStructureRepository == null || _unitOfWork == null) return NotFound();

        var orgStructure = new OrgStructure { Id = id };
        try
        {
            _orgStructureRepository.Delete(orgStructure);
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
