using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Entities;

namespace OrganizationalStructure.Models
{
    public class OrgStructureContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<OrgStructure> OrgStructures { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<User> Users { get; set; }

        public OrgStructureContext(DbContextOptions<OrgStructureContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
    }
}
