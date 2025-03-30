using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class DocumentTagBindingModel : IDocumentTagModel
    {
        public int DocumentId { get; set; }

        public int TagId { get; set; }

        public string Value { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
