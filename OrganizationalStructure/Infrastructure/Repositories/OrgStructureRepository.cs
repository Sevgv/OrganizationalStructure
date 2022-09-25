using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Infrastructure.Repositories
{
    public class OrgStructureRepository : Repository<OrgStructure>, IOrgStructureRepository
    {
        public OrgStructureRepository(OrgStructureContext context) 
            : base(context)
        {
        }
    }
}
