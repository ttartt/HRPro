using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class MessageInfoViewModel : IMessageInfoModel
    {
        public string MessageId { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public DateTime DateDelivery { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
