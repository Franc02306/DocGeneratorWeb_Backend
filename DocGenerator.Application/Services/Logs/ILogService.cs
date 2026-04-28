using DocGenerator.Domain.Enums;

namespace DocGenerator.Application.Services.Logs
{
    public interface ILogService
    {
        /// <summary>
        /// Escribe un registro de log de forma asíncrona.
        /// </summary>
        Task WriteAsync(LogEntry entry);
    }
}
