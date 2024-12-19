using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IUserModel : IId
    {
        int? CompanyId { get; }
        string Surname { get; }
        string Name { get; }
        string? LastName { get; }
        string Email { get; }
        string Password { get; }
        string? PhoneNumber { get; }
        DateTime DateOfBirth { get; }
        string? Gender { get; }
        string? City { get; }
        RoleEnum Role { get; }
    }
}
