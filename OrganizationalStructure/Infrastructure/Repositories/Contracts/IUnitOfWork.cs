namespace OrganizationalStructure.Infrastructure.Repositories.Contracts;

public interface IUnitOfWork
{
    Task Commit();
}
