using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface ITagModel : IId
    {
        string TagName { get; }
        string Name { get; }
        DataTypeEnum Type { get; }
        int TemplateId { get; }
    }
}
