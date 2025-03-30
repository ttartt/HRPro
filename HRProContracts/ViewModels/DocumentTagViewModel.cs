using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class DocumentTagViewModel : IDocumentTagModel
    {
        public int DocumentId { get; set; }

        public int TagId { get; set; }

        public string Value { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
