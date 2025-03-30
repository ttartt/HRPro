using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HRProDatabaseImplement.Models
{
    [DataContract]
    public class Task : ITaskModel
    {
        [DataMember]
        [Required]
        public int TestTaskId { get; set; }
        [DataMember]
        [Required]
        public string Text { get; set; } = string.Empty;
        [DataMember]
        public string? Image { get; set; }
        [DataMember]
        public string? Comment { get; set; }
        [DataMember]
        public int Id { get; set; }

        public virtual TestTask TestTask { get; set; }

        public static Task? Create(TaskBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new Task
            {
                Id = model.Id,
                TestTaskId = model.TestTaskId,
                Text = model.Text,
                Image = model.Image,
                Comment = model.Comment
            };
        }

        public static Task Create(TaskViewModel model)
        {
            return new Task
            {
                Id = model.Id,
                TestTaskId = model.TestTaskId,
                Text = model.Text,
                Image = model.Image,
                Comment = model.Comment
            };
        }

        public void Update(TaskBindingModel model)
        {
            if (model == null)
            {
                return;
            }
            Text = model.Text;
            Image = model.Image;
            Comment = model.Comment;
        }

        public TaskViewModel GetViewModel => new()
        {
            Id = Id,
            TestTaskId = TestTaskId,
            Text = Text,
            Image = Image,
            Comment = Comment
        };
    }
}
