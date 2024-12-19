using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class UserViewModel : IUserModel
    {
        public int? CompanyId { get; set; }

        public string Surname { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public RoleEnum Role { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int Id { get; set; }

        public List<DocumentViewModel> Documents { get; set; } = new();

        public string? City { get; set; }
    }
}
