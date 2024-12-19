using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface ITestTaskModel : IId
    {
        int CreatorId { get; }
        string Topic { get; }
        TestTaskStatusEnum Status { get; }
    }
}
