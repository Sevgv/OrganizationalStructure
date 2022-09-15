using OrganizationalStructure.Entities;

namespace OrganizationalStructure.Models
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            SecondName = user.SecondName;
            MiddleName = user.MiddleName;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
    }
}
