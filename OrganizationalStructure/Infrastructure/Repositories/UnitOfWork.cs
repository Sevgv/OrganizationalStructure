using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrgStructureContext _context;
    public UnitOfWork(OrgStructureContext context)
    {
        _context = context;
    }

    public async Task Commit() => await _context.SaveChangesAsync();
}
