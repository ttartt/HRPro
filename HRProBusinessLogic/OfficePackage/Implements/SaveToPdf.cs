using HRProBusinessLogic.OfficePackage.HelperEnums;
using HRProBusinessLogic.OfficePackage.HelperModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace HRProBusinessLogic.OfficePackage.Implements
{
    public class SaveToPdf : AbstractSaveToPdf
    {
        private Document? _document;
        private Section? _section;
        private static ParagraphAlignment
        GetParagraphAlignment(PdfParagraphAlignmentType type)
        {
            return type switch
            {
                PdfParagraphAlignmentType.Center => ParagraphAlignment.Center,
                PdfParagraphAlignmentType.Left => ParagraphAlignment.Left,
                PdfParagraphAlignmentType.Right => ParagraphAlignment.Right,
                _ => ParagraphAlignment.Justify,
            };
        }

        private static void DefineStyles(Document document)
        {
            var style = document.Styles["Normal"];
            style.Font.Name = "Times New Roman";
            style.Font.Size = 14;
            style.Font.Color = Colors.Black;

            style = document.Styles.AddStyle("NormalTitle", "Normal");
            style.Font.Size = 16;
            style.Font.Bold = true;

            style = document.Styles.AddStyle("Subtitle", "Normal");
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "0.5cm";
            style.ParagraphFormat.SpaceAfter = "0.2cm";

            document.DefaultPageSetup.TopMargin = "2.5cm";
            document.DefaultPageSetup.BottomMargin = "2.5cm";
            document.DefaultPageSetup.LeftMargin = "2cm";
            document.DefaultPageSetup.RightMargin = "2cm";
        }

        protected override void CreatePdf(PdfInfo info)
        {
            _document = new Document();
            DefineStyles(_document);
            _section = _document.AddSection();
        }
        protected override void CreateParagraph(PdfParagraph pdfParagraph)
        {
            if (_section == null)
            {
                return;
            }
            var paragraph = _section.AddParagraph(pdfParagraph.Text);
            paragraph.Format.SpaceAfter = "1cm";
            paragraph.Format.Alignment = GetParagraphAlignment(pdfParagraph.ParagraphAlignment);
            paragraph.Style = pdfParagraph.Style;
        }

        protected override void SavePdf(PdfInfo info)
        {
            var renderer = new PdfDocumentRenderer(true)
            {
                Document = _document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(info.FileName);
        }
    }
}
