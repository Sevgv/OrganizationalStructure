using OrganizationalStructure.Models.ImportModels;

namespace OrganizationalStructure.Infrastructure.Repositories.Contracts
{
    public interface IImportRepository
    {
        Task InitializeDatabase();
        IEnumerable<OrgStructureDto> GetOrgStructureDto(string filePath);
    }
}
