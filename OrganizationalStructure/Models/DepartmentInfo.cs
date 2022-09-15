using OrganizationalStructure.Entities;

namespace OrganizationalStructure.Models
{
    public class DepartmentInfo
    {
        public DepartmentInfo() { }

        public DepartmentInfo(OrgStructure orgStructure)
        {
            Id = orgStructure.Id;
            Name = orgStructure.Department.Name;
            UserCount = orgStructure.Department.OrgStructures
                .Select(x => x.User).Distinct().Count();
            PositionCount = orgStructure.Department.OrgStructures
                .Select(x => x.Position).Distinct().Count();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int PositionCount { get; set; }       
    }
}
