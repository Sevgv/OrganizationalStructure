using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Infrastructure.Repositories
{
    public class PositionRepository : Repository<Position>, IPositionRepository
    {
        public PositionRepository(OrgStructureContext context) 
            : base(context)
        {
        }
    }
}
