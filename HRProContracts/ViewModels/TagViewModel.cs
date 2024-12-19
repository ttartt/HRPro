using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.ViewModels
{
    public class TagViewModel : ITagModel
    {
        public string TagName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DataTypeEnum Type { get; set; }

        public int TemplateId { get; set; }

        public int Id { get; set; }

        public string? TemplateName { get; set; }
    }
}
