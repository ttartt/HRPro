using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface ITagModel : IId
    {
        string TagName { get; }
        DataTypeEnum Type { get; }
        int TemplateId { get; }
    }
}
