using OrganizationalStructure.Entities;

namespace OrganizationalStructure.Models
{
    public class PositionModel
    {
        public PositionModel() { }
        public PositionModel(Position position)
        {
            Id = position.Id;
            Name = position.Name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
