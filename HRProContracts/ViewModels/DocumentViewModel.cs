using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class DocumentViewModel : IDocumentModel
    {
        public int CreatorId { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DocumentStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? TemplateId { get; set; }

        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? CreatorName { get; set; }

        public string? TemplateName { get; set; }

        public List<string> Tags { get; set; }
    }
}
