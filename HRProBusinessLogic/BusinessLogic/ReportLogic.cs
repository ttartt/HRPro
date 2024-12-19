using HRProBusinessLogic.OfficePackage;
using HRProBusinessLogic.OfficePackage.HelperModels;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;

namespace HRProBusinessLogic.BusinessLogic
{
    public class ReportLogic : IReportLogic
    {
        private readonly IResumeStorage _resumeStorage;
        private readonly IUserStorage _userStorage;
        private readonly IVacancyStorage _vacancyStorage;
        private readonly AbstractSaveToPdf _saveToPdf;
        public ReportLogic(IResumeStorage resumeStorage, AbstractSaveToPdf saveToPdf, IUserStorage userStorage, IVacancyStorage vacancyStorage)
        {
            _resumeStorage = resumeStorage;
            _saveToPdf = saveToPdf;
            _userStorage = userStorage;
            _vacancyStorage = vacancyStorage;
        }

        public ResumeViewModel GetResume(ReportBindingModel model)
        {
            var resume = _resumeStorage.GetElement(new ResumeSearchModel
            {
                Id = model.ResumeId
            });
            resume.UserName = _userStorage.GetElement(new UserSearchModel { Id = resume.UserId }).Surname + " " + _userStorage.GetElement(new UserSearchModel { Id = resume.UserId }).Name + " " + _userStorage.GetElement(new UserSearchModel { Id = resume.UserId }).LastName;
            resume.VacancyName = _vacancyStorage.GetElement(new VacancySearchModel { Id = resume.VacancyId }).JobTitle;
            if (resume != null)
            {
                return resume;
            }
            return null;
        }

        public void SaveResumeToPdf(ReportBindingModel model)
        {
            _saveToPdf.CreateDocReportResume(new PdfInfo
            {
                FileName = model.FileName,
                Title = GetResume(model).VacancyName,
                Resume = GetResume(model)
            });
        }

        public List<ResumeViewModel> GetResumesStatistics(ReportBindingModel model)
        {
            var list = _resumeStorage.GetFilteredList(new ResumeSearchModel { VacancyId = model.VacancyId }).Where(resume =>
                (!model.DateFrom.HasValue || resume.CreatedAt >= model.DateFrom.Value) &&
                (!model.DateTo.HasValue || resume.CreatedAt <= model.DateTo.Value)).ToList();

            foreach (var item in list)
            {
                item.UserName = _userStorage.GetElement(new UserSearchModel { Id = item.UserId }).Surname + " " + _userStorage.GetElement(new UserSearchModel { Id = item.UserId }).Name + " " + _userStorage.GetElement(new UserSearchModel { Id = item.UserId }).LastName;
            }
            
            if (list != null)
            {
                return list;
            }
            return null;
        }

        public void SaveResumesStatisticsToPdf(ReportBindingModel model)
        {
            _saveToPdf.CreateDocStatistics(new PdfInfo
            {
                FileName = model.FileName,
                Title = _vacancyStorage.GetElement(new VacancySearchModel { Id = model.VacancyId }).JobTitle,
                Resumes = GetResumesStatistics(model),
                DateFrom = model.DateFrom,
                DateTo = model.DateTo
            });
        }
    }
}
