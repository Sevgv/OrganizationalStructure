using OrganizationalStructure.Entities;

namespace OrganizationalStructure.Models
{
    public class OrgStructureModel
    {
        public OrgStructureModel() { }

        public OrgStructureModel(OrgStructure orgStructure)
        {
            Id = orgStructure.Id;
            DepartmentId = orgStructure.DepartmentId;
            DepartmentName = orgStructure.Department?.Name ?? string.Empty;
            PositionId = orgStructure.PositionId;
            PositionName = orgStructure.Position?.Name ?? string.Empty;
            UserId = orgStructure.UserId;
            UserName = $"{orgStructure.User.SecondName} {orgStructure.User.FirstName} {orgStructure.User.MiddleName}";
        }

        public Guid Id { get; set; }

        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public Guid PositionId { get; set; }
        public string PositionName { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }       
    }
}
