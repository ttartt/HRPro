using HRProDataModels.Enums;
using HRProDataModels.Models;

namespace HRProContracts.BindingModels
{
    public class TagBindingModel : ITagModel
    {
        public string TagName { get; set; } = string.Empty;

        public DataTypeEnum Type { get; set; }

        public int TemplateId { get; set; }

        public int Id { get; set; }
    }
}
