using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication6.Providers;

namespace WebApplication6.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ObserverOptions _options;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IOptions<ObserverOptions> options, ILogger<NotificationService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task Notify(string message, Stream picture)
        {
            try
            {
                var smtpClient = new SmtpClient(_options.Smpt, _options.SmtpPort);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_options.Login, _options.Password);

                var mailMessage = new MailMessage(_options.Login, _options.ObserverEmail, "info", $"reference : {message}");
                if (picture != null)
                {
                    mailMessage.Attachments.Add(new Attachment(picture, "picture.png", "image/png"));
                }

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("mail is sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"attempt to send email fails. {ex.Message}");
            }
        }
    }
}
