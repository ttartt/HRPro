using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class ResponsibilityViewModel : IResponsibilityModel
    {
        public string Name { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}
