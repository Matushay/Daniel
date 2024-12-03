using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Proyect.Servicios
{
    public class WelcomeEmailService
    {
        private readonly EmailSettings _emailSettings;

        public WelcomeEmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName, string resetLink)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Tu Empresa", _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress(userName, toEmail));
            emailMessage.Subject = "¡Bienvenido a Nuestra Plataforma!";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
<body>
    <h1>Hola, {userName}!</h1>
    <p>Por favor, haz clic en el siguiente enlace para establecer tu contraseña:</p>
    <a href='{resetLink}'>Establecer Contraseña</a>
</body>
</html>"
            };

            emailMessage.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, false);
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }

}
