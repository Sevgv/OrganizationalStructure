using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(OrgStructureContext context) 
            : base(context)
        {
        }
    }
}
