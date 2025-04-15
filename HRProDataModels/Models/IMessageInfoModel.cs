namespace HRProDataModels.Models
{
    public interface IMessageInfoModel
    {
        string MessageId { get; }
        int? UserId { get; }
        string SenderName { get; }
        DateTime DateDelivery { get; }
        string Subject { get; }
        string Body { get; }
    }
}
