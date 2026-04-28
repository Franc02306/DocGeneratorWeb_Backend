using DocGenerator.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DocGenerator.Application.Services.EmailNotifications
{
    public class EmailNotification : IEmailNotification
    {
        private readonly SmtpSettings _smtp;
        private readonly DocGeneratorWebSettings _web;

        public EmailNotification(IOptions<SmtpSettings> smtp, IOptions<DocGeneratorWebSettings> web)
        {
            _smtp = smtp.Value;
            _web = web.Value;
        }

        /// <summary>
        /// Enviar un token de verificación para el usuario recién creado
        /// </summary>
        public async Task SendUserVerificationToken(string userName, string email)
        {
            var subject = "Verificación de cuenta";

            var templatePath = Path.Combine(
                AppContext.BaseDirectory,
                _smtp.EmailTemplates,
                "VerifyUser.html"
            );

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"No se encontró la plantilla de correo: {templatePath}");

            var htmlBody = await File.ReadAllTextAsync(templatePath);

            // Base URL según entorno
            var baseUrl = _web.IsProd ? _web.ProdLink : _web.DevLink;

            // Aquí puedes luego agregar token real si quieres
            var verificationLink = $"{baseUrl}/verify-account?user={userName}";

            htmlBody = htmlBody
                .Replace("{{UserName}}", userName)
                .Replace("{{VerificationLink}}", verificationLink);

            await SendEmailAsync(email, subject, htmlBody);
        }

        /// <summary>
        /// Enviar correo base
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(_smtp.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _smtp.Host,
                _smtp.Port,
                _smtp.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto
            );

            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
