using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Entities;
using OrganizationalStructure.Models;

namespace OrganizationalStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly OrgStructureContext _context;

        public DepartmentsController(OrgStructureContext context)
        {
            _context = context;
        }

        // GET: api/Departments/{Guid}
        [HttpGet("GetDepartmentsByParentId/{parentId}")]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartmentsByParentId(Guid parentId)
        {
            if (_context.Departments == null)
            {
                return NotFound();
            }
            return (await _context.Departments.Where(x => x.ParentDepartmentId == parentId).ToListAsync())
                .Select(x => new DepartmentModel(x)).ToList();
        }

        // GET: api/Departments/{Guid}
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentModel>> GetDepartment(Guid id)
        {
            if (_context.Departments == null)
            {
                return NotFound();
            }
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return new DepartmentModel(department);
        }

        // PUT: api/Departments/{Guid}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(Guid id, Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            var dep = await _context.Departments.FindAsync(department.Id);
            if (dep == null) return NotFound();

            dep.Name = !string.IsNullOrEmpty(department.Name) ? department.Name : dep.Name;
            dep.ParentDepartment = await _context.Departments.FindAsync(department.ParentDepartmentId) 
                ?? dep.ParentDepartment;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DepartmentModel>> PostDepartment(Department department)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'OrgStructureContext.Departments'  is null.");
            }

            var dep = new Department { Id = Guid.NewGuid() };

            try
            {
                if (string.IsNullOrEmpty(department.Name)) return BadRequest("Не указано название отдела");
                dep.Name = department.Name;
                dep.ParentDepartment = await _context.Departments.FindAsync(department.ParentDepartmentId);
                
                _context.Departments.Add(dep);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return CreatedAtAction("GetDepartment", new { id = dep.Id }, new DepartmentModel(dep));
        }

        // DELETE: api/Departments/{Guid}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            if (_context.Departments == null)
            {
                return NotFound();
            }
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var childDepartments = await _context.Departments.Where(x => x.ParentDepartment == department).ToListAsync();
            childDepartments.ForEach(x => 
            {
                x.ParentDepartmentId = null;
                x.ParentDepartment = null;
            });

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(Guid id)
        {
            return (_context.Departments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
