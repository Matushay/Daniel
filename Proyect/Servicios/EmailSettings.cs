using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace Proyect.Servicios
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string FromEmail { get; set; }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress(string.Empty, toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = body };

            // Email body with HTML and CSS styling
            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                <html>
                <head>
                    <style>
                        .btn {{
                            display: inline-block;
                            padding: 10px 10px;
                            margin: 20px 0;
                            font-size: 16px;
                            color: #000;
                            background-color: #90ee90;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .btn i {{
                            margin-right: 5px;
                        }}
                    </style>
                </head>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='text-align: center; margin: 20px;'>
                        <h2 style='color: #333;'>Recuperación de contraseña</h2>
                        <p>Por favor, restablezca su contraseña haciendo clic en el botón de abajo:</p>
                        <a href='{body}' class='btn' >
                            <i class='bi bi-link-45deg'></i> Restablecer Contraseña
                        </a>
                        <p>Si no solicitó un cambio de contraseña, ignore este mensaje.</p>
                    </div>
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
