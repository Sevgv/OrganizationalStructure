using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;
using System.Linq.Expressions;

namespace OrganizationalStructure.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected OrgStructureContext _context;
    protected Repository(OrgStructureContext context)
    {
        _context = context;
    }

    public virtual async Task<T> AddAsync(T entity) => 
        (await _context.AddAsync(entity)).Entity;

    public virtual async Task AddManyAsync(IEnumerable<T> entities) => 
        await _context.Set<T>().AddRangeAsync(entities);

    public virtual void Delete(T entity) =>
        _context.Remove(entity);

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) =>
        await _context.Set<T>()
        .AsQueryable()
        .Where(predicate)
        .ToListAsync();

    public virtual async Task<IEnumerable<T>> GetAllAsync() => 
        await _context.Set<T>().ToListAsync();

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _context.FindAsync<T>(id);
        return entity ?? throw new ArgumentException($"Entity with {id} doesn't exists");
    }

    public virtual async Task<bool> IsExists(Expression<Func<T, bool>> predicate) => 
        await _context.Set<T>().AnyAsync(predicate);  
}
