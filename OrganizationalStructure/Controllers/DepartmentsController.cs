using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Extensions;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.DepartmentModels;
using OrganizationalStructure.Validators;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly DepartmentValidator _validator;

    public DepartmentsController(
        IDepartmentRepository departmentRepository, 
        IUnitOfWork unitOfWork, 
        DepartmentValidator validator)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;

        _validator = validator;
    }

    // GET: api/Departments/{Guid}
    /// <summary>
    /// Gets departments by parent department only
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    [HttpGet("GetDepartmentsByParentId/{parentId}")]
    public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartmentsByParentId(Guid parentId)
    {
        if (_departmentRepository == null) return NotFound();

        return (await _departmentRepository
            .GetAsync(department => department.ParentDepartmentId == parentId))
            .Select(x => new DepartmentModel(x)).ToList();
    }

    // GET: api/Departments/{Guid}
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentModel>> GetDepartment(Guid id)
    {
        if (_departmentRepository == null) return NotFound();
        
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null) return NotFound();
        
        return new DepartmentModel(department);
    }

    // PUT: api/Departments/{Guid}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDepartment(Guid id, Department department)
    {       
        if (id != department.Id) return BadRequest();
        if (_departmentRepository == null || _unitOfWork == null) return NotFound();

        var results = _validator.Validate(department);
        if (!results.IsValid)
            return ValidationProblem(results.Errors.GetStringErrorMessange());

        var dep = await _departmentRepository.GetByIdAsync(department.Id);
        if (dep == null) return NotFound();

        dep.Name = !string.IsNullOrEmpty(department.Name) ? department.Name : dep.Name;
        dep.ParentDepartment = department.ParentDepartmentId != null 
            ? await _departmentRepository.GetByIdAsync((Guid)department.ParentDepartmentId)
            : null;

        try
        {
            await _unitOfWork.Commit();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _departmentRepository.IsExists(x => x.Id == id)) return NotFound();
            else throw;           
        }

        return NoContent();
    }

    // POST: api/Departments
    [HttpPost]
    public async Task<ActionResult<DepartmentModel>> PostDepartment(Department department)
    {
        if (_departmentRepository == null || _unitOfWork == null) return NotFound();

        var newDepartment = new Department { Id = Guid.NewGuid() };

        try
        {
            var results = _validator.Validate(department);
            if (!results.IsValid)
                return ValidationProblem(results.Errors.GetStringErrorMessange());

            newDepartment.Name = department.Name;
            // If the parent department is null,
            // then the depatment becomes the root department
            newDepartment.ParentDepartment = department.ParentDepartmentId != null 
                ? await _departmentRepository.GetByIdAsync((Guid)department.ParentDepartmentId)
                : null;
           
            newDepartment = await _departmentRepository.AddAsync(newDepartment);
            await _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return CreatedAtAction("GetDepartment", new { id = newDepartment.Id }, new DepartmentModel(newDepartment));
    }

    // DELETE: api/Departments/{Guid}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        if (_departmentRepository == null || _unitOfWork == null) return NotFound();

        var department = new Department { Id = id };

        try
        {
            _departmentRepository.Delete(department);
            await _unitOfWork.Commit();
        }
        catch(DbUpdateConcurrencyException e)
        {
            return NotFound($"Record does not exist in the database. Message: {e.Message}");
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }


        return NoContent();
    }

    // GET: api/Departments/GetDepartmentsInfo
    /// <summary>
    /// Displays information on the number of employees in each department and the number of positions in that department
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("GetDepartmentsInfo")]
    public async Task<ActionResult<IEnumerable<DepartmentInfo>>> GetDepartmentsInfo()
    {
        if (_departmentRepository == null) return NotFound();

        var departments = await _departmentRepository.GetAllAsync();
        if (departments == null || !departments.Any()) return NotFound();

        return departments.Select(x => new DepartmentInfo(x)).ToList();
    }
}
