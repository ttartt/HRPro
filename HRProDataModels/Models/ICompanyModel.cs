namespace HRProDataModels.Models
{
    public interface ICompanyModel : IId
    {
        string Name { get; }
        string? LogoFilePath { get; }
        string? Description { get; }
        string? Website { get; }
        string? Address { get; }
        string? Contacts { get; }
    }
}
