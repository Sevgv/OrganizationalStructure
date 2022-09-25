namespace OrganizationalStructure.Domain;

public class User : Entity
{
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;

    public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();
}

