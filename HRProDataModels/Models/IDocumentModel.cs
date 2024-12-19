using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface IDocumentModel : IId
    {
        int CreatorId { get; }
        int CompanyId { get; }
        string Name { get; }
        DocumentStatusEnum Status { get; }
        DateTime CreatedAt { get; }
        int TemplateId { get; }
    }
}
