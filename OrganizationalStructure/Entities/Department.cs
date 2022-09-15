namespace OrganizationalStructure.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid? ParentDepartmentId { get; set; }
        public virtual Department? ParentDepartment { get; set; }

        public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}