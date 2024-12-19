using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface ITagModel : IId
    {
        string Tag { get; }
        string Name { get; }
        DataTypeEnum Type { get; }
        int TemplateId { get; }
    }
}
