using HRProBusinessLogic.OfficePackage.HelperEnums;
using HRProBusinessLogic.OfficePackage.HelperModels;

namespace HRProBusinessLogic.OfficePackage
{
    public abstract class AbstractSaveToPdf
    {
        public void CreateDocStatistics(PdfInfo info)
        {
            CreatePdf(info);

            CreateParagraph(new PdfParagraph
            {
                Text = $"Статистика по резюме на вакансию: {info.Title}",
                Style = "NormalTitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Center
            });            

            int totalResumes = info.Resumes.Count;

            CreateParagraph(new PdfParagraph
            {
                Text = $"Общее количество резюме: {totalResumes}",
                Style = "Normal",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            foreach (var resume in info.Resumes)
            {
                CreateParagraph(new PdfParagraph
                {
                    Text = $"- Кандидат: {resume.CandidateInfo}, Дата создания: {resume.CreatedAt:dd.MM.yyyy}",
                    Style = "Normal",
                    ParagraphAlignment = PdfParagraphAlignmentType.Left
                });
            }
            SavePdf(info);
        }

        public void CreateDocReportResume(PdfInfo info)
        {
            CreatePdf(info);

            CreateParagraph(new PdfParagraph
            {
                Text = $"Резюме: {info.Title}",
                Style = "NormalTitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Center
            });

            CreateParagraph(new PdfParagraph
            {
                Text = $"О кандидате: {info.Resume.CandidateInfo ?? "Не указано"}",
                Style = "Normal",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            CreateParagraph(new PdfParagraph
            {
                Text = $"Вакансия: {info.Resume.VacancyName ?? "Не указано"}",
                Style = "Normal",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            CreateParagraph(new PdfParagraph
            {
                Text = "Образование:",
                Style = "Subtitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });            

            CreateParagraph(new PdfParagraph
            {
                Text = "Опыт работы:",
                Style = "Subtitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            CreateParagraph(new PdfParagraph
            {
                Text = "Навыки:",
                Style = "Subtitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            CreateParagraph(new PdfParagraph
            {
                Text = "Описание:",
                Style = "Subtitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Left
            });

            SavePdf(info);
        }

        protected abstract void CreatePdf(PdfInfo info);
        protected abstract void CreateParagraph(PdfParagraph paragraph);
        protected abstract void SavePdf(PdfInfo info);
    }
}
