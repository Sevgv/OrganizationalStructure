using OrganizationalStructure.Domain;

namespace OrganizationalStructure.Models.DepartmentModels;

public class DepartmentModel
{
    public DepartmentModel(Department department)
    {
        Id = department.Id;
        Name = department.Name;
    }
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
