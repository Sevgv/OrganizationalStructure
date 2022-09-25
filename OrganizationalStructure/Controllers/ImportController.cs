using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.OrgStructureModels;

namespace OrganizationalStructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportController : ControllerBase
{
    private readonly IOrgStructureRepository _orgStructureRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IImportRepository _importRepository;
    private readonly IUserRepository _userRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public ImportController(
        IOrgStructureRepository orgStructureRepository, 
        IDepartmentRepository departmentRepository, 
        IPositionRepository positionRepository, 
        IImportRepository importRepository, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork)
    {
        _orgStructureRepository = orgStructureRepository;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _importRepository = importRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Deletes data from the database and writes new data from the file
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <returns>List of imported objects</returns>
    [HttpPost]
    public async Task<ActionResult<IEnumerable<OrgStructureModel>>> ImportFromFileToDB(string filePath)
    {
        try
        {
            var data = _importRepository.GetOrgStructureDto(filePath);
            if (data == null || !data.Any()) return NotFound();

            var orgStructures = data.Select(x => new OrgStructure(x)).ToList();
            if (orgStructures == null || !orgStructures.Any()) return NotFound();

            var users = orgStructures.Select(x => x.User)
                .DistinctBy(x => $"{x.FirstName}{x.SecondName}{x.MiddleName}")
                .ToList();
            var positions = orgStructures.Select(x => x.Position)
                .DistinctBy(x => x.Name)
                .ToList();
            var departments = orgStructures.Select(x => x.Department)
                .DistinctBy(x => $"{x.Name}{x.ParentDepartment?.Name ?? ""}")
                .ToList();

            await _importRepository.CleareDatabase();

            users.ForEach(user => user.Id = Guid.NewGuid());
            await _userRepository.AddManyAsync(users);         
            
            positions.ForEach(position => position.Id = Guid.NewGuid());
            await _positionRepository.AddManyAsync(positions);

            departments.ForEach(department =>
            {
                department.Id = Guid.NewGuid();
                department.Departments = departments
                    .Where(x => x.ParentDepartment != null && x.ParentDepartment.Name == department.Name)
                    .ToList();
                department.ParentDepartment = null;
            });
            await _departmentRepository.AddManyAsync(departments);          

            orgStructures.ForEach(structure =>
            {
                var position = positions.First(x => x.Name == structure.Position.Name);
                structure.PositionId = position.Id;
                structure.Position = position;

                var department = departments.First(x => x.Name == structure.Department.Name);
                structure.DepartmentId = department.Id;
                structure.Department = department;

                var user = users
                    .First(x => $"{x.FirstName}{x.SecondName}{x.MiddleName}" == $"{structure.User.FirstName}{structure.User.SecondName}{structure.User.MiddleName}");
                structure.UserId = user.Id;
                structure.User = user;
            });

            await _orgStructureRepository.AddManyAsync(orgStructures);
            await _unitOfWork.Commit();

            return (await _orgStructureRepository.GetAllAsync())
                .Select(x => new OrgStructureModel(x))
                .ToList();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
