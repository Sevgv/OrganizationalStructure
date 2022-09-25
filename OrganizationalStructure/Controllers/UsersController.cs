using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.UserModels;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
    {
        if (_userRepository == null) return NotFound();

        return (await _userRepository.GetAllAsync())
            .Select(x => new UserModel(x)).ToList();
    }

    // GET: api/Users/{Guid}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUser(Guid id)
    {
        if (_userRepository == null) return NotFound();

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)return NotFound();

        return new UserModel(user);
    }

    // PUT: api/Users/{Guid}
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(Guid id, User user)
    {
        if (id != user.Id) return BadRequest();
        if (_userRepository == null || _unitOfWork == null) return NotFound();

        var u = await _userRepository.GetByIdAsync(user.Id);
        if (u == null) return NotFound();

        u.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : u.FirstName;
        u.SecondName = !string.IsNullOrEmpty(user.SecondName) ? user.SecondName : u.SecondName;
        u.MiddleName = !string.IsNullOrEmpty(user.MiddleName) ? user.MiddleName : u.MiddleName;

        try
        {
            await _unitOfWork.Commit();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _userRepository.IsExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // POST: api/Users
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<UserModel>> PostUser(User user)
    {
        if (_userRepository == null || _unitOfWork == null) return NotFound();

        var newUser = new User { Id = Guid.NewGuid() };

        try
        {
            if (string.IsNullOrEmpty(user.FirstName)) return BadRequest("Не указано имя");
            newUser.FirstName = user.FirstName;
            if (string.IsNullOrEmpty(user.SecondName)) return BadRequest("Не указана фамилия");
            newUser.SecondName = user.SecondName;
            if (string.IsNullOrEmpty(user.MiddleName)) return BadRequest("Не указано отчество");
            newUser.MiddleName = user.MiddleName;

            newUser = await _userRepository.AddAsync(newUser);
            await _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return CreatedAtAction("GetUser", new { id = newUser.Id }, new UserModel(newUser));
    }

    // DELETE: api/Users/{Guid}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (_userRepository == null || _unitOfWork == null) return NotFound();

        var user = new User { Id = id };

        try
        {
            _userRepository.Delete(user);
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
