using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class DocumentBindingModel : IDocumentModel
    {
        public int CreatorId { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DocumentStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TemplateId { get; set; }

        public int Id { get; set; }
    }
}
