using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Candidate : ICandidateModel
    {
        public int? TestTaskId { get; set; }
        [Required]
        public string FIO { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Id { get; set; }
        public virtual TestTask? TestTask { get; set; }

        public static Candidate? Create(CandidateBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Candidate()
            {
                Id = model.Id,
                TestTaskId = model.TestTaskId,
                FIO = model.FIO,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
        }

        public static Candidate Create(CandidateViewModel model)
        {
            return new Candidate
            {
                Id = model.Id,
                TestTaskId = model.TestTaskId,
                FIO = model.FIO,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
        }

        public void Update(CandidateBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            TestTaskId = model.TestTaskId;
            FIO = model.FIO;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
        }

        public CandidateViewModel GetViewModel => new()
        {
            Id = Id,
            TestTaskId = TestTaskId,
            FIO = FIO,
            Email = Email,
            PhoneNumber = PhoneNumber
        };
    }
}
