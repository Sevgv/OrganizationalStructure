using OrganizationalStructure.Domain;
using System.Linq.Expressions;

namespace OrganizationalStructure.Infrastructure.Repositories.Contracts;

public interface IRepository<T> where T : Entity
{
    /// <summary>
    /// Begins tracking the given entity as added
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Tracking entity</returns>
    Task<T> AddAsync(T entity);
    /// <summary>
    /// Begins tracking the given entities as added
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task AddManyAsync(IEnumerable<T> entities);
    /// <summary>
    /// Begins tracking the given entity as deleted
    /// </summary>
    /// <param name="entity"></param>
    void Delete(T entity);
    /// <summary>
    /// Check for exists by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>True if exists any entity satisfying the predicate, else false</returns>
    Task<bool> IsExists(Expression<Func<T, bool>> predicate);
    /// <summary>
    /// Get all entities in the table database
    /// </summary>
    /// <returns>All entities in the table database</returns>
    Task<IEnumerable<T>> GetAllAsync();
    /// <summary>
    /// Get entities by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>Entities satisfying the predicate</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
    /// <summary>
    /// Get entity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetByIdAsync(Guid id);   
}
