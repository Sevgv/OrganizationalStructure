namespace OrganizationalStructure.Entities
{
    public class Position
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();
    }
}