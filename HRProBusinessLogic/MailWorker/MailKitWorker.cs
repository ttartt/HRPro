using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using System.Security.Authentication;
using System.Text;
using MailKit.Net.Pop3;
using MailKit.Security;

namespace HRProBusinessLogic.MailWorker
{
    public class MailKitWorker : AbstractMailWorker
    {
        public MailKitWorker(ILogger<MailKitWorker> logger, IUserLogic userLogic) : base(logger, userLogic) { }

        protected override async Task SendMailAsync(MailSendInfoBindingModel info)
        {
            using var objMailMessage = new MailMessage();
            using var objSmtpClient = new SmtpClient(_smtpClientHost, _smtpClientPort);

            try
            {
                objMailMessage.From = new MailAddress(_mailLogin);
                objMailMessage.To.Add(new MailAddress(info.MailAddress));
                objMailMessage.Subject = info.Subject;
                objMailMessage.Body = info.Text;
                objMailMessage.SubjectEncoding = Encoding.UTF8;
                objMailMessage.BodyEncoding = Encoding.UTF8;

                objSmtpClient.UseDefaultCredentials = false;
                objSmtpClient.EnableSsl = true;
                objSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                objSmtpClient.Credentials = new NetworkCredential(_mailLogin, _mailPassword);

                await Task.Run(() => objSmtpClient.Send(objMailMessage));
            }
            catch (Exception)
            {
                throw;
            }
        }        
    }
}
