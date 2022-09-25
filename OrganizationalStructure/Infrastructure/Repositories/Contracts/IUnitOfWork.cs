namespace OrganizationalStructure.Infrastructure.Repositories.Contracts;

public interface IUnitOfWork
{
    /// <summary>
    /// Save changes in the database
    /// </summary>
    /// <returns></returns>
    Task Commit();
}
