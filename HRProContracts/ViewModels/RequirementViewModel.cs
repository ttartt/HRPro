using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class RequirementViewModel : IRequirementModel
    {
        public string Name { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
