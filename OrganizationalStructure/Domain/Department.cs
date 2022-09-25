namespace OrganizationalStructure.Domain;

public class Department : Entity
{
    public string Name { get; set; } = string.Empty;

    public Guid? ParentDepartmentId { get; set; }
    public virtual Department? ParentDepartment { get; set; }

    public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
