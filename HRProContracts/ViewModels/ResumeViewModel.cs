using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class ResumeViewModel : IResumeModel
    {
        public int VacancyId { get; set; }

        public string VacancyName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Experience { get; set; } = string.Empty;

        public string Education { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string Skills { get; set; } = string.Empty;

        public ResumeStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Id { get; set; }
    }
}
