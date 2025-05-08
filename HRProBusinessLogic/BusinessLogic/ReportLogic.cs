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
                .Where(x => x.SalaryRange.From.HasValue || x.SalaryRange.To.HasValue)
                .ToList();

            return new SalaryStatisticsViewModel
            {
                AverageSalaryByPosition = vacanciesWithSalary
                    .GroupBy(v => v.JobTitle)
                    .ToDictionary(
                        g => g.Key,
                        g => CalculateAverageSalary(g.Select(x => x.SalaryRange))),

                SalaryRangesByPosition = vacanciesWithSalary
                    .GroupBy(v => v.JobTitle)
                    .ToDictionary(
                        g => g.Key,
                        g => CalculateSalaryRange(g.Select(x => x.SalaryRange))) 
            };
        }

        private decimal CalculateAverageSalary(IEnumerable<(decimal? From, decimal? To)> ranges)
        {
            var validSalaries = ranges
                .Select(r => r.From ?? r.To ?? 0) 
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

            salaryString = salaryString.ToLower().Replace("руб.", "").Replace("рублей", "").Trim();

            var rangeMatch = Regex.Match(salaryString, @"от\s*([\d\s,]+)\s*до\s*([\d\s,]+)");
            if (rangeMatch.Success)
            {
                return (ParseNumber(rangeMatch.Groups[1].Value),
                       ParseNumber(rangeMatch.Groups[2].Value));
            }

            var fromMatch = Regex.Match(salaryString, @"от\s*([\d\s,]+)");
            if (fromMatch.Success)
            {
                return (ParseNumber(fromMatch.Groups[1].Value), null);
            }

            var toMatch = Regex.Match(salaryString, @"до\s*([\d\s,]+)");
            if (toMatch.Success)
            {
                return (null, ParseNumber(toMatch.Groups[1].Value));
            }

            var simpleMatch = Regex.Match(salaryString, @"^([\d\s,]+)$");
            if (simpleMatch.Success)
            {
                var val = ParseNumber(simpleMatch.Groups[1].Value);
                return (val, val);
            }

            return (null, null);
        }

        private decimal? ParseNumber(string numberStr)
        {
            if (string.IsNullOrWhiteSpace(numberStr))
                return null;

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

        public ResumeStatisticsViewModel GetResumeStatistics()
        {
            var resumes = _resumeStorage.GetFullList();

            var cityNames = new Dictionary<string, string>
            {
                {"ulyanovsk", "Ульяновск"},
                {"samara", "Самара"},
                {"moskva", "Москва"},
                {"kazan", "Казань"},
                {"sankt-peterburg", "Санкт-Петербург"},
                {"novosibirsk", "Новосибирск"}
            };

            var resumesWithSalary = resumes
                .Select(r => new {
                    r.Title,
                    City = cityNames.TryGetValue(r.City ?? "", out var name) ? name : r.City,
                    ParsedSalary = ParseSalary(r.Salary)
                })
                .Where(x => x.ParsedSalary.HasValue)
                .ToList();

            return new ResumeStatisticsViewModel
            {
                AverageSalaryByCity = resumesWithSalary
                    .Where(r => !string.IsNullOrEmpty(r.City))
                    .GroupBy(r => r.City)
                    .Where(g => g.Count() >= 2) 
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(r => r.ParsedSalary.Value)),

                SalaryByTitle = resumesWithSalary
                    .GroupBy(r => r.Title)
                    .Where(g => g.Count() >= 2) 
                    .ToDictionary(
                        g => g.Key,
                        g => new SalaryStatsViewModel
                        {
                            Average = g.Average(r => r.ParsedSalary.Value),
                            Count = g.Count(),
                            Min = g.Min(r => r.ParsedSalary.Value),
                            Max = g.Max(r => r.ParsedSalary.Value)
                        }),

                TotalResumes = resumes.Count,
                ResumesWithSalary = resumesWithSalary.Count,
                ResumesWithoutSalary = resumes.Count - resumesWithSalary.Count
            };
        }

        private decimal? ParseSalary(string salaryString)
        {
            if (string.IsNullOrWhiteSpace(salaryString))
                return null;

            var cleaned = Regex.Replace(salaryString, @"[^\d,.]", "");

            cleaned = cleaned.Replace(",", ".");

            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {                
                return result;
            }

            return null;
        }
    }
}
