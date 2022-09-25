using OrganizationalStructure.Domain;
using System.Linq.Expressions;

namespace OrganizationalStructure.Infrastructure.Repositories.Contracts;

public interface IRepository<T> where T : Entity
{
    Task<T> AddAsync(T entity);
    Task AddManyAsync(IEnumerable<T> entities);
    void Delete(T entity);
    Task<bool> IsExists(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(Guid id);   
}
