using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Proyect.Servicios
{
    public class SendGridEmailService
    {
        private readonly string _apiKey;

        public SendGridEmailService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("medellinsalvaje@gmail.com", "Medellin Salvaje");
            var to = new EmailAddress(toEmail);

            // Usando los dos tipos de contenido (texto plano y HTML)
            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent,
                htmlContent
            );

            // Enviar el correo
            await client.SendEmailAsync(msg);
        }
    }
}
