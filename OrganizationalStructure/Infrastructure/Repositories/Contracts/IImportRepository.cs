using OrganizationalStructure.Models.ImportModels;

namespace OrganizationalStructure.Infrastructure.Repositories.Contracts
{
    public interface IImportRepository
    {
        Task CleareDatabase();
        IEnumerable<OrgStructureDto> GetOrgStructureDto(string filePath);
    }
}
