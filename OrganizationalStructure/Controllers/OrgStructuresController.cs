using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Entities;
using OrganizationalStructure.Models;

namespace OrganizationalStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrgStructuresController : ControllerBase
    {
        private readonly OrgStructureContext _context;

        public OrgStructuresController(OrgStructureContext context)
        {
            _context = context;
        }

        // GET: api/OrgStructures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrgStructureModel>>> GetOrgStructures()
        {
            if (_context.OrgStructures == null)
            {
                return NotFound();
            }
            return (await _context.OrgStructures.ToListAsync())
                .Select(structure => new OrgStructureModel(structure)).ToList();
        }

        // GET: api/OrgStructures/{Guid}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrgStructureModel>> GetOrgStructure(Guid id)
        {
            if (_context.OrgStructures == null)
            {
                return NotFound();
            }
            var orgStructure = await _context.OrgStructures.FindAsync(id);

            if (orgStructure == null)
            {
                return NotFound();
            }

            return new OrgStructureModel(orgStructure);
        }

        // PUT: api/OrgStructures/{Guid}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrgStructure(Guid id, OrgStructure orgStructure)
        {
            if (id != orgStructure.Id)
            {
                return BadRequest();
            }

            try
            {

                var structure = await _context.OrgStructures.FindAsync(orgStructure.Id);
                if (structure == null) return NotFound();

                structure.Position = await _context.Positions.FindAsync(orgStructure.PositionId) ?? structure.Position;
                structure.Department = await _context.Departments.FindAsync(orgStructure.DepartmentId) ?? structure.Department;
                structure.User = await _context.Users.FindAsync(orgStructure.UserId) ?? structure.User;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrgStructureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        // POST: api/OrgStructures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrgStructureModel>> PostOrgStructure(OrgStructure orgStructure)
        {
            if (_context.OrgStructures == null)
            {
                return Problem("Entity set 'OrgStructureContext.OrgStructures'  is null.");
            }

            var structure = new OrgStructure { Id = Guid.NewGuid() };

            try
            {
                structure.Position = await _context.Positions.FindAsync(orgStructure.PositionId) 
                    ?? throw new BadHttpRequestException("Профессия не указана или такой не существует");
                structure.Department = await _context.Departments.FindAsync(orgStructure.DepartmentId) 
                    ?? throw new BadHttpRequestException("Отдел не указан или такого не существует");
                structure.User = await _context.Users.FindAsync(orgStructure.UserId) 
                    ?? throw new BadHttpRequestException("Пользователь не указан или или такого не существует");

                _context.OrgStructures.Add(structure);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrgStructure), new { id = structure.Id }, new OrgStructureModel(structure));
            }
            catch (BadHttpRequestException e)
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
            if (_context.OrgStructures == null)
            {
                return NotFound();
            }
            var orgStructure = await _context.OrgStructures.FindAsync(id);
            if (orgStructure == null)
            {
                return NotFound();
            }

            _context.OrgStructures.Remove(orgStructure);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("GetDepartmentsInfo")]
        public async Task<ActionResult<IEnumerable<DepartmentInfo>>> GetDepartmentsInfo()
        {
            if (_context.OrgStructures == null)
            {
                return NotFound();
            }

            var structures = (await _context.OrgStructures.ToListAsync()).DistinctBy(x => x.Department).ToList();
            if (structures == null || !structures.Any()) return NotFound();

            return structures.Select(x => new DepartmentInfo(x)).ToList();
        }

        private bool OrgStructureExists(Guid id)
        {
            return (_context.OrgStructures?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
