using OrganizationalStructure.Domain;

namespace OrganizationalStructure.Models.DepartmentModels;

public class DepartmentInfo
{
    public DepartmentInfo() { }

    public DepartmentInfo(Department department)
    {
        var orgStructers = department.OrgStructures;

        Name = department.Name;
        UserCount = orgStructers
            .Select(x => x.User).Distinct().Count();
        PositionCount = orgStructers
            .Select(x => x.Position).Distinct().Count();
    }

    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int PositionCount { get; set; }
}
