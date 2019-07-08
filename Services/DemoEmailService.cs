using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Services
{
    public class DemoEmailService : IEmailService
    {
        private readonly IHostingEnvironment _environment;

        public DemoEmailService(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task SendAsync(string email, string name, string to, string subject, string body, string file = null)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = $@"{_environment.WebRootPath}\MailDump";
                var message = new MailMessage
                {
                    Body = body,
                    Subject = subject,
                    From = new MailAddress(email, name),
                    IsBodyHtml = true
                };
                message.To.Add($"{to}@cognizant.com");
                if (!string.IsNullOrWhiteSpace(file))
                {
                    message.Attachments.Add(new Attachment(file));
                }

                await smtp.SendMailAsync(message);
            }
        }
    }
}
