using HRProDataModels.Enums;

namespace HRProContracts.SearchModels
{
    public class UserSearchModel
    {
        public int? CompanyId { get; set; }

        public string? Surname { get; set; }

        public string? Name { get; set; } 

        public string? Email { get; set; }

        public RoleEnum? Role { get; set; }

        public string? Password { get; set; } 

        public int? Id { get; set; }
    }
}
