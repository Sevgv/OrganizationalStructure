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

    public async virtual Task<T> AddAsync(T entity) => 
        (await _context.AddAsync(entity)).Entity;

    public async virtual Task AddManyAsync(IEnumerable<T> entities) => 
        await _context.Set<T>().AddRangeAsync(entities);   

    public virtual void  Delete(T entity) => 
        _context.Remove(entity);

    public async virtual Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) =>
        await _context.Set<T>()
        .AsQueryable()
        .Where(predicate)
        .ToListAsync();

    public async virtual Task<IEnumerable<T>> GetAllAsync() => 
        await _context.Set<T>().ToListAsync();

    public async virtual Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _context.FindAsync<T>(id);
        return entity ?? throw new ArgumentException($"Entity with {id} doesn't exists");
    }

    public async virtual Task<bool> IsExists(Guid id) => 
        await _context.Set<T>().AnyAsync(e => e.Id == id);  
}
