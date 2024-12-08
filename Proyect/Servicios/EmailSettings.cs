using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Proyect.Models;

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
        private object usuario;

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

            // Email body with HTML and CSS styling
            var builder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px;
            text-align: center;
        }}
        .logo-container {{
            margin-bottom: 20px;
        }}
        .logo {{
            max-width: 150px; /* Asegúrate de que la imagen se ajuste */
        }}
        .email-header {{
            font-size: 24px;
            font-weight: bold;
            color: #333333;
            margin-bottom: 20px;
        }}
        .email-content {{
            font-size: 16px;
            color: #555555;
            line-height: 1.6;
            margin-bottom: 30px;
        }}
        .btn {{
            display: inline-block;
            padding: 12px 20px;
            font-size: 16px;
            color: white; /* Color de la letra blanco */
            background-color: #f1f3f4;
            text-decoration: none;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
            transition: background-color 0.3s ease;
        }}
        .btn:hover {{
            background-color: #e0e0e0;
        }}
        .footer {{
            font-size: 12px;
            color: #888888;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='logo-container'>
            <!-- Asegúrate de que la URL del logo sea correcta -->
            <img src='https://imgur.com/a/FFbmozo' class='logo'/>
        </div>
        <div class='email-header'>Recuperación de contraseña</div>
        <div class='email-content'>
            Hola <strong></strong>,<br><br>
            Por favor, restablezca su contraseña haciendo clic en el botón de abajo:
        </div>
        <a href='{body}' class='btn btn-primary text-white'>Restablecer Contraseña</a>
        <br />
        <br />

        <div class='email-content'>
            Si no solicitó un cambio de contraseña, ignore este mensaje.
        </div>
        <div class='footer'>© 2024 Medellin Salvaje. Todos los derechos reservados.</div>
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
