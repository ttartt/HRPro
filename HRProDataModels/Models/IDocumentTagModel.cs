namespace HRProDataModels.Models
{
    public interface IDocumentTagModel : IId
    {
        int DocumentId { get; }
        int TagId { get; }
        string Value { get; }
    }
}
