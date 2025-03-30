using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRproDatabaseImplement.Models
{
    [DataContract]
    public class User : IUserModel
    {
        [DataMember]
        public int? CompanyId { get; set; }
        [Required]
        [DataMember]
        public string Surname { get; set; } = string.Empty;
        [DataMember]
        [Required]
        public string Name { get; set; } = string.Empty;
        [DataMember]
        public string? LastName { get; set; }
        [DataMember]
        [Required]
        public string Email { get; set; } = string.Empty;
        [DataMember]
        [Required]
        public string Password { get; set; } = string.Empty;
        [DataMember]
        public string? PhoneNumber { get; set; }
        [DataMember]
        [Required]
        public RoleEnum Role { get; set; }
        [DataMember]
        public DateTime? DateOfBirth { get; set; }
        [DataMember]
        public string? City { get; set; }
        [DataMember]
        public int Id { get; set; }
        public virtual Company Company { get; set; }

        public static User? Create(UserBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new User()
            {
                Id = model.Id,
                Surname = model.Surname,
                Name = model.Name,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CompanyId = model.CompanyId,
                DateOfBirth = model.DateOfBirth.HasValue ? model.DateOfBirth.Value.ToUniversalTime().AddHours(4) : null,
                City = model.City
            };
        }
        public static User Create(UserViewModel model)
        {
            return new User
            {
                Id = model.Id,
                Surname = model.Surname,
                Name = model.Name,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                CompanyId = model.CompanyId,
                DateOfBirth = model.DateOfBirth.HasValue ? model.DateOfBirth.Value.ToUniversalTime().AddHours(4) : null
            };
        }
        public void Update(UserBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Surname = model.Surname;
            Name = model.Name;
            LastName = model.LastName;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
            CompanyId = model.CompanyId;
            DateOfBirth = model.DateOfBirth.HasValue ? model.DateOfBirth.Value.ToUniversalTime().AddHours(4) : null;
        }

        public UserViewModel GetViewModel => new()
        {
            Id = Id,
            Name = Name,
            Surname = Surname,
            LastName = LastName,
            Email = Email,
            Password = Password,
            PhoneNumber = PhoneNumber,
            Role = Role,
            CompanyId = CompanyId,
            DateOfBirth = DateOfBirth,
            City = City
        };
    }
}
