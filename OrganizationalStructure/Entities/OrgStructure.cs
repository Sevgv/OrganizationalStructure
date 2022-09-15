using OrganizationalStructure.Models;

namespace OrganizationalStructure.Entities
{
    public class OrgStructure
    {
        public OrgStructure() { }

        public OrgStructure(OrgStructureDto orgStructureDto)
        {
            Id = Guid.NewGuid();
            Department = new Department
            {
                Name = orgStructureDto.Department,
                ParentDepartment = new Department
                {
                    Name = orgStructureDto.ParentDepartment
                }
            };
            Position = new Position
            {
                Name = orgStructureDto.Position
            };

            var fio = orgStructureDto.User.Trim().Split(' ');

            User = new User
            {
                SecondName = fio[0],
                FirstName = fio[1],
                MiddleName = fio[2]
            };
        }

        public Guid Id { get; set; }

        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; } = new Department();

        public Guid PositionId { get; set; }
        public virtual Position Position { get; set; } = new Position();

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = new User();       
    }
}
