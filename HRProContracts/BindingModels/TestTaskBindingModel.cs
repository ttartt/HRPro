using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class TestTaskBindingModel : ITestTaskModel
    {
        public int CreatorId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public TestTaskStatusEnum Status { get; set; }

        public int Id { get; set; }
    }
}
