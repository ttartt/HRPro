using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class TaskBindingModel : ITaskModel
    {
        public int TestTaskId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? Image { get; set; }

        public string? Comment { get; set; }

        public int Id { get; set; }
    }
}
