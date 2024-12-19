using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Enums;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class TestTask : ITestTaskModel
    {
        [Required]
        public int CreatorId { get; set; }

        [Required]
        public string Topic { get; set; } = string.Empty;

        [Required]
        public TestTaskStatusEnum Status { get; set; }

        public int Id { get; set; }
        public virtual User Creator { get; set; }

        public static TestTask? Create(TestTaskBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new TestTask()
            {
                Id = model.Id,
                CreatorId = model.CreatorId,
                Topic = model.Topic,
                Status = model.Status
            };
        }

        public static TestTask Create(TestTaskViewModel model)
        {
            return new TestTask
            {
                Id = model.Id,
                CreatorId = model.CreatorId,
                Topic = model.Topic,
                Status = model.Status
            };
        }

        public void Update(TestTaskBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Topic = model.Topic;
            Status = model.Status;
        }

        public TestTaskViewModel GetViewModel => new()
        {
            Id = Id,
            CreatorId = CreatorId,
            Topic = Topic,
            Status = Status
        };
    }
}
