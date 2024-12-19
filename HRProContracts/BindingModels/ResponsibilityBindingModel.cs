using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class ResponsibilityBindingModel : IResponsibilityModel
    {
        public string Name { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
