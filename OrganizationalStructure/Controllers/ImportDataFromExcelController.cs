using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OrganizationalStructure.Entities;
using OrganizationalStructure.Extensions;
using OrganizationalStructure.Models;

namespace OrganizationalStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportDataFromExcelController : ControllerBase
    {
        private readonly OrgStructureContext _context;

        public ImportDataFromExcelController(OrgStructureContext context)
        {
            _context = context;
        }

        private IEnumerable<OrgStructureDto> GetAllDataFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return new List<OrgStructureDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            var newcollection = worksheet.ConvertSheetToObjects<OrgStructureDto>().ToList();

            return newcollection;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<OrgStructureModel>>> ImportFromFileToDB(string filePath)
        {
            try
            {
                var data = GetAllDataFromFile(filePath);
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

                if (!_context.Users.Any())
                {
                    foreach (var user in users)
                    {
                        user.Id = Guid.NewGuid();
                    }

                    await _context.Users.AddRangeAsync(users);
                }

                if (!_context.Positions.Any())
                {
                    foreach (var position in positions)
                    {
                        position.Id = Guid.NewGuid();
                    }

                    await _context.Positions.AddRangeAsync(positions);
                }

                if (!_context.Departments.Any())
                {
                    foreach (var department in departments)
                    {
                        department.Id = Guid.NewGuid();
                    }
                    foreach (var department in departments)
                    {
                        if (department.ParentDepartment?.Name == null)
                        {
                            department.ParentDepartment = null;
                            continue;
                        }

                        department.ParentDepartmentId = departments
                            .First(x => x.Name == department.ParentDepartment.Name).Id;
                        department.ParentDepartment = null;
                    }

                    await _context.Departments.AddRangeAsync(departments);
                }

                await _context.SaveChangesAsync();


                if (!_context.OrgStructures.Any())
                {
                    orgStructures.ForEach(structure =>
                    {
                        var position = _context.Positions.First(x => x.Name == structure.Position.Name);
                        structure.PositionId = position.Id;
                        structure.Position = position;

                        var department = _context.Departments.First(x => x.Name == structure.Department.Name);
                        structure.DepartmentId = department.Id;
                        structure.Department = department;

                        var user = _context.Users.AsEnumerable()
                            .First(x => $"{x.FirstName}{x.SecondName}{x.MiddleName}" == $"{structure.User.FirstName}{structure.User.SecondName}{structure.User.MiddleName}");
                        structure.UserId = user.Id;
                        structure.User = user;
                    });

                    await _context.OrgStructures.AddRangeAsync(orgStructures);
                }

                await _context.SaveChangesAsync();

                return (await _context.OrgStructures.ToListAsync()).Select(x => new OrgStructureModel(x)).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
