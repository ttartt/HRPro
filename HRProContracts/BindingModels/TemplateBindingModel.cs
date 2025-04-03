using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class TemplateBindingModel : ITemplateModel
    {
        public string Name { get; set; } = string.Empty;

        public TemplateTypeEnum Type { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public int Id { get; set; }
        public int? CompanyId { get; set; }
    }
}
