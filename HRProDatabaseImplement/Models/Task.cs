using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using HRProDataModels.Models;
using System.ComponentModel.DataAnnotations;

namespace HRProDatabaseImplement.Models
{
    public class Task : ITaskModel
    {
        [Required]
        public int TestTaskId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string? Image { get; set; }
        public string? Comment { get; set; }

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
