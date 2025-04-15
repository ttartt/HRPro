using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using Microsoft.Extensions.Logging;

namespace HRProBusinessLogic.MailWorker
{
    public abstract class AbstractMailWorker
    {
        protected string _mailLogin = string.Empty;
        protected string _mailPassword = string.Empty;
        protected string _smtpClientHost = string.Empty;
        protected int _smtpClientPort;
        protected string _popHost = string.Empty;
        protected int _popPort;
        private readonly IMessageInfoLogic _messageInfoLogic;
        private readonly IUserLogic _clientLogic;
        private readonly ILogger _logger;

        public AbstractMailWorker(ILogger<AbstractMailWorker> logger, IMessageInfoLogic messageInfoLogic, IUserLogic clientLogic)
        {
            _logger = logger;
            _messageInfoLogic = messageInfoLogic;
            _clientLogic = clientLogic;
        }

        public void MailConfig(MailConfigBindingModel config)
        {
            _mailLogin = config.MailLogin;
            _mailPassword = config.MailPassword;
            _smtpClientHost = config.SmtpClientHost;
            _smtpClientPort = config.SmtpClientPort;
            _popHost = config.PopHost;
            _popPort = config.PopPort;
            _logger.LogDebug("Config: {login}, {password}, {clientHost}, {clientPOrt}, {popHost}, {popPort}", _mailLogin, _mailPassword, _smtpClientHost, _smtpClientPort, _popHost, _popPort);
        }

        public async void MailSendAsync(MailSendInfoBindingModel info)
        {
            if (string.IsNullOrEmpty(_mailLogin) || string.IsNullOrEmpty(_mailPassword))
            {
                return;
            }

            if (string.IsNullOrEmpty(_smtpClientHost) || _smtpClientPort == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(info.MailAddress) || string.IsNullOrEmpty(info.Subject) || string.IsNullOrEmpty(info.Text))
            {
                return;
            }

            _logger.LogDebug("Send Mail: {To}, {Subject}", info.MailAddress, info.Subject);

            await SendMailAsync(info);
        }

        public async void MailCheck()
        {
            if (string.IsNullOrEmpty(_mailLogin) || string.IsNullOrEmpty(_mailPassword))
            {
                return;
            }

            if (string.IsNullOrEmpty(_popHost) || _popPort == 0)
            {
                return;
            }

            if (_messageInfoLogic == null)
            {
                return;
            }

            var list = await ReceiveMailAsync();
            _logger.LogDebug("Check Mail: {Count} new mails", list.Count);

            foreach (var mail in list)
            {
                mail.UserId = _clientLogic.ReadElement(new() { Email = mail.SenderName })?.Id;
                _messageInfoLogic.Create(mail);
            }
        }

        protected abstract Task SendMailAsync(MailSendInfoBindingModel info);
        protected abstract Task<List<MessageInfoBindingModel>> ReceiveMailAsync();
    }
}
