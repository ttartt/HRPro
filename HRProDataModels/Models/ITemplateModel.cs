using HRProDataModels.Enums;

namespace HRProDataModels.Models
{
    public interface ITemplateModel : IId
    {
        string Name { get; }
        TemplateTypeEnum Type { get; }
        string FilePath { get; }
    }
}
