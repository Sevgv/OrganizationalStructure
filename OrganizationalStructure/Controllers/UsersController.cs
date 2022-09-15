using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Entities;
using OrganizationalStructure.Models;

namespace OrganizationalStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OrgStructureContext _context;

        public UsersController(OrgStructureContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return (await _context.Users.ToListAsync())
                .Select(x => new UserModel(x)).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new UserModel(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var u = await _context.Users.FindAsync(user.Id);
            if (u == null) return NotFound();

            u.FirstName = !string.IsNullOrEmpty(u.FirstName) ? user.FirstName : u.FirstName;
            u.SecondName = !string.IsNullOrEmpty(u.SecondName) ? user.SecondName : u.SecondName;
            u.MiddleName = !string.IsNullOrEmpty(u.MiddleName) ? user.MiddleName : u.MiddleName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'OrgStructureContext.Users'  is null.");
            }
            var u = new User { Id = Guid.NewGuid() };

            try
            {
                if (string.IsNullOrEmpty(user.FirstName)) return BadRequest("Не указано имя");
                u.FirstName = user.FirstName;
                if (string.IsNullOrEmpty(user.SecondName)) return BadRequest("Не указана фамилия");
                u.SecondName = user.SecondName;
                if (string.IsNullOrEmpty(user.MiddleName)) return BadRequest("Не указано отчество");
                u.MiddleName = user.MiddleName;

                _context.Users.Add(u);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return CreatedAtAction("GetUser", new { id = u.Id }, new UserModel(u));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
