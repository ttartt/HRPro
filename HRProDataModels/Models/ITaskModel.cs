namespace HRProDataModels.Models
{
    public interface ITaskModel : IId
    {
        int TestTaskId { get; }
        string Text { get; }
        string? Image { get; }
        string? Comment { get; }
    }
}
