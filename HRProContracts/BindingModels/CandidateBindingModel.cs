using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class CandidateBindingModel : ICandidateModel
    {
        public int? TestTaskId { get; set; }

        public string FIO { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int Id { get; set; }
    }
}
