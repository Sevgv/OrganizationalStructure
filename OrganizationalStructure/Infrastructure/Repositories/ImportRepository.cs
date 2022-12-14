using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OrganizationalStructure.Extensions;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using OrganizationalStructure.Models.ImportModels;

namespace OrganizationalStructure.Infrastructure.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly OrgStructureContext _context;
        public ImportRepository(OrgStructureContext context)
        {
            _context = context;
        }

        public IEnumerable<OrgStructureDto> GetOrgStructureDto(string filePath)
        {
            if (!File.Exists(filePath)) return new List<OrgStructureDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            var newcollection = worksheet.ConvertSheetToObjects<OrgStructureDto>().ToList();

            return newcollection;
        }

        public async Task CleareDatabase()
        {
            await _context.Database.ExecuteSqlRawAsync("" +
                "TRUNCATE TABLE public.\"OrgStructures\" CASCADE;\r\n" +
                "TRUNCATE TABLE public.\"Departments\" CASCADE;\r\n" +
                "TRUNCATE TABLE public.\"Positions\" CASCADE;\r\n" +
                "TRUNCATE TABLE public.\"Users\" CASCADE;");
        }
    }
}
