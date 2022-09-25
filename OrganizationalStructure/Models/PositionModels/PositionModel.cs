using OrganizationalStructure.Domain;

namespace OrganizationalStructure.Models.PositionModels;

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
