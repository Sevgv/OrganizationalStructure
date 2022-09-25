using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain;

namespace OrganizationalStructure.Infrastructure;

public class OrgStructureContext : DbContext
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<OrgStructure> OrgStructures => Set<OrgStructure>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<User> Users => Set<User>();

    public OrgStructureContext(DbContextOptions<OrgStructureContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Departments)
            .WithOne(pd => pd.ParentDepartment)
            .HasForeignKey(k => k.ParentDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
