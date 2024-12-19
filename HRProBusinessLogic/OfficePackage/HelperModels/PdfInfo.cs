using HRProContracts.ViewModels;

namespace HRProBusinessLogic.OfficePackage.HelperModels
{
    public class PdfInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public ResumeViewModel Resume { get; set; } = new();
        public List<ResumeViewModel> Resumes { get; set; } = new();
    }
}
