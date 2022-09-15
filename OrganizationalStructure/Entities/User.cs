namespace OrganizationalStructure.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;

        public virtual ICollection<OrgStructure> OrgStructures { get; set; } = new List<OrgStructure>();
    }
}
