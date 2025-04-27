using HRProBusinessLogic.OfficePackage;
using HRProBusinessLogic.OfficePackage.HelperModels;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

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

        // новые методы
        // получение статистики по вилке зарплат
        public SalaryStatisticsViewModel GetSalaryStatistics()
        {
            var vacancies = _vacancyStorage.GetFullList();

            var vacanciesWithSalary = vacancies
                .Select(v => new {
                    v.JobTitle,
                    SalaryRange = ParseSalaryRange(v.Salary)
                })
                .Where(x => x.SalaryRange.From.HasValue || x.SalaryRange.To.HasValue) // Изменили условие
                .ToList();

            return new SalaryStatisticsViewModel
            {
                AverageSalaryByPosition = vacanciesWithSalary
                    .GroupBy(v => v.JobTitle)
                    .ToDictionary(
                        g => g.Key,
                        g => CalculateAverageSalary(g.Select(x => x.SalaryRange))), // Новый метод расчета

                SalaryRangesByPosition = vacanciesWithSalary
                    .GroupBy(v => v.JobTitle)
                    .ToDictionary(
                        g => g.Key,
                        g => CalculateSalaryRange(g.Select(x => x.SalaryRange))) // Новый метод расчета
            };
        }

        private decimal CalculateAverageSalary(IEnumerable<(decimal? From, decimal? To)> ranges)
        {
            var validSalaries = ranges
                .Select(r => r.From ?? r.To ?? 0) // Берем From, если нет - To, если нет - 0 (но такие будут отфильтрованы)
                .Where(s => s > 0)
                .ToList();

            return validSalaries.Any() ? validSalaries.Average() : 0;
        }

        private SalaryRangeViewModel CalculateSalaryRange(IEnumerable<(decimal? From, decimal? To)> ranges)
        {
            var fromValues = ranges.Where(r => r.From.HasValue).Select(r => r.From.Value).ToList();
            var toValues = ranges.Where(r => r.To.HasValue).Select(r => r.To.Value).ToList();

            var allValues = fromValues.Concat(toValues).ToList();

            return new SalaryRangeViewModel
            {
                Min = allValues.Any() ? allValues.Min() : 0,
                Max = allValues.Any() ? allValues.Max() : 0,
                Average = allValues.Any() ? allValues.Average() : 0
            };
        }

        // перевод зарплаты в формате "от ... до ..." в числовой формат
        private (decimal? From, decimal? To) ParseSalaryRange(string salaryString)
        {
            if (string.IsNullOrWhiteSpace(salaryString))
                return (null, null);

            // Нормализация строки
            salaryString = salaryString.ToLower().Replace("руб.", "").Replace("рублей", "").Trim();

            // 1. Вилка "от X до Y"
            var rangeMatch = Regex.Match(salaryString, @"от\s*([\d\s,]+)\s*до\s*([\d\s,]+)");
            if (rangeMatch.Success)
            {
                return (ParseNumber(rangeMatch.Groups[1].Value),
                       ParseNumber(rangeMatch.Groups[2].Value));
            }

            // 2. Только "от X"
            var fromMatch = Regex.Match(salaryString, @"от\s*([\d\s,]+)");
            if (fromMatch.Success)
            {
                return (ParseNumber(fromMatch.Groups[1].Value), null);
            }

            // 3. Только "до Y"
            var toMatch = Regex.Match(salaryString, @"до\s*([\d\s,]+)");
            if (toMatch.Success)
            {
                return (null, ParseNumber(toMatch.Groups[1].Value));
            }

            // 4. Просто число
            var simpleMatch = Regex.Match(salaryString, @"^([\d\s,]+)$");
            if (simpleMatch.Success)
            {
                var val = ParseNumber(simpleMatch.Groups[1].Value);
                return (val, val);
            }

            return (null, null);
        }

        // перевод зарплаты в формате "..." в числовой формат
        private decimal? ParseNumber(string numberStr)
        {
            if (string.IsNullOrWhiteSpace(numberStr))
                return null;

            // Удаляем все пробелы и заменяем запятые на точки (для дробных чисел)
            var cleaned = numberStr.Replace(" ", "").Replace(",", ".");

            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }

        public VacancyStatusStatisticsViewModel GetVacancyStatusStatistics()
        {
            var vacancies = _vacancyStorage.GetFullList();

            return new VacancyStatusStatisticsViewModel
            {
                StatusCounts = vacancies
                    .GroupBy(v => v.Status.ToString()) 
                    .OrderBy(g => g.Key) 
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()),

                TotalVacancies = vacancies.Count,
                StatusPercentages = vacancies
                    .GroupBy(v => v.Status.ToString())
                    .ToDictionary(
                        g => g.Key,
                        g => Math.Round((double)g.Count() / vacancies.Count * 100, 1))
            };
        }

        /*public DocumentStatisticsViewModel GetDocumentStatistics(ReportBindingModel model)
        {
            // Здесь нужен доступ к хранилищу документов
            // Предполагаем наличие IDocumentStorage
            var documents = _documentStorage.GetFilteredList(new DocumentSearchModel
            {
                DateFrom = model.DateFrom,
                DateTo = model.DateTo
            });

            return new DocumentStatisticsViewModel
            {
                TotalDocuments = documents.Count,
                DocumentsByType = documents
                    .GroupBy(d => d.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),

                // Дополнительная статистика по дням/неделям
                DailyCounts = documents
                    .GroupBy(d => d.CreatedAt.Date)
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }*/
    }
}
