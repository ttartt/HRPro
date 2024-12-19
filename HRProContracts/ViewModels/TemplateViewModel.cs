using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class TemplateViewModel : ITemplateModel
    {
        public string Name { get; set; } = string.Empty;

        public TemplateTypeEnum Type { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public int Id { get; set; }

        public List<TagViewModel> Tags { get; set; } = new();
    }
}
