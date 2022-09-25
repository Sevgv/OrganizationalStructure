namespace OrganizationalStructure.Domain;

public class Position : Entity
{
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();
}
