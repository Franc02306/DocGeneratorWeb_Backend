namespace DocGenerator.Application.Services.EmailNotifications
{
    public interface IEmailNotification
    {
        /// <summary>
        /// Enviar un token de verificación para el usuario recíén creado
        /// </summary>
        Task SendUserVerificationToken(string userName, string email);
    }
}
