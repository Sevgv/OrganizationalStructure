using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.DepartmentModels;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentsController(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    // GET: api/Departments/{Guid}
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

    // TODO: Задокументировать, что при передаче ParentId == null, отдел становится корневым, а не записывается старое значение
    // PUT: api/Departments/{Guid}
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDepartment(Guid id, Department department)
    {
        if (id != department.Id) return BadRequest();
        if (_departmentRepository == null || _unitOfWork == null) return NotFound();

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
            if (!await _departmentRepository.IsExists(id)) return NotFound();
            else throw;           
        }

        return NoContent();
    }

    // POST: api/Departments
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<DepartmentModel>> PostDepartment(Department department)
    {
        if (_departmentRepository == null || _unitOfWork == null) return NotFound();

        var newDepartment = new Department { Id = Guid.NewGuid() };

        try
        {
            if (string.IsNullOrEmpty(department.Name)) return BadRequest("Не указано название отдела");
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
