using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class RequirementBindingModel : IRequirementModel
    {
        public string Name { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
