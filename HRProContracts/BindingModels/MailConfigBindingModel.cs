namespace HRProContracts.BindingModels
{
    public class MailConfigBindingModel
    {
        public string MailLogin { get; set; } = string.Empty;
        public string MailPassword { get; set; } = string.Empty;
        public string SmtpClientHost { get; set; } = string.Empty;
        public int SmtpClientPort { get; set; }
        public string PopHost { get; set; } = string.Empty;
        public int PopPort { get; set; }
    }
}
