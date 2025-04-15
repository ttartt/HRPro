using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Message
    {
        [Key]
        public string MessageId { get; private set; } = string.Empty;
        public int? UserId { get; private set; }
        [Required]
        public string SenderName { get; private set; } = string.Empty;
        [Required]
        public DateTime DateDelivery { get; private set; } = DateTime.Now;
        [Required]
        public string Subject { get; private set; } = string.Empty;
        [Required]
        public string Body { get; private set; } = string.Empty;

        public virtual User User { get; set; }

        public static Message? Create(MessageInfoBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new()
            {
                Body = model.Body,
                Subject = model.Subject,
                UserId = model.UserId,
                MessageId = model.MessageId,
                SenderName = model.SenderName,
                DateDelivery = model.DateDelivery,
            };
        }

        public MessageInfoViewModel GetViewModel => new()
        {
            Body = Body,
            Subject = Subject,
            UserId = UserId,
            MessageId = MessageId,
            SenderName = SenderName,
            DateDelivery = DateDelivery,
        };
    }
}
