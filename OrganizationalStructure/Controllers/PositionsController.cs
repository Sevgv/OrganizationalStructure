using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Entities;
using OrganizationalStructure.Models;

namespace OrganizationalStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly OrgStructureContext _context;

        public PositionsController(OrgStructureContext context)
        {
            _context = context;
        }

        // GET: api/Positions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionModel>>> GetPositions()
        {
            if (_context.Positions == null)
            {
                return NotFound();
            }
            return (await _context.Positions.ToListAsync())
                .Select(x => new PositionModel(x)).ToList();
        }

        // GET: api/Positions/{Guid}
        [HttpGet("{id}")]
        public async Task<ActionResult<PositionModel>> GetPosition(Guid id)
        {
            if (_context.Positions == null)
            {
                return NotFound();
            }
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
            {
                return NotFound();
            }

            return new PositionModel(position);
        }

        // PUT: api/Positions/{Guid}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(Guid id, Position position)
        {
            if (id != position.Id)
            {
                return BadRequest();
            }

            var pos = await _context.Positions.FindAsync(position.Id);
            if (pos == null) return NotFound();

            pos.Name = !string.IsNullOrEmpty(pos.Name) ? position.Name : pos.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        // POST: api/Positions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PositionModel>> PostPosition(Position position)
        {
            if (_context.Positions == null)
            {
                return Problem("Entity set 'OrgStructureContext.Positions'  is null.");
            }

            var pos = new Position { Id = Guid.NewGuid() };

            try
            {
                if (string.IsNullOrEmpty(position.Name)) return BadRequest("Не указано название професси");
                pos.Name = position.Name;

                _context.Positions.Add(pos);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
           
            return CreatedAtAction("GetPosition", new { id = pos.Id }, new PositionModel(pos));
        }

        // DELETE: api/Positions/{Guid}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(Guid id)
        {
            if (_context.Positions == null)
            {
                return NotFound();
            }
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionExists(Guid id)
        {
            return (_context.Positions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
