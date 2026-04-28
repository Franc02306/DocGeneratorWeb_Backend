using DocGenerator.Domain.Enums;

namespace DocGenerator.Application.Services.Logs
{
    public class LogService : ILogService
    {
        /// <summary>
        /// Escribe un log en un archivo diario organizado por módulo.
        /// </summary>
        public async Task WriteAsync(LogEntry entry)
        {
            var baseDir = Path.Combine(AppContext.BaseDirectory, "Logs");

            var moduleDir = Path.Combine(baseDir, entry.Module);
            Directory.CreateDirectory(moduleDir);

            var filePath = Path.Combine(moduleDir, $"{entry.Module.ToLower()}-{DateTime.Now:yyyy-MM-dd}.log");

            var line =
                $"[{entry.Date:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{entry.Module}] [{entry.Method}] [{entry.Path}] " +
                $"[{entry.Action}] [{entry.StatusCode}] " +
                $"[User={entry.User ?? "anon"}] " +
                $"[Request={entry.RequestBody ?? "<empty>"}] " +
                $"{(string.IsNullOrWhiteSpace(entry.ResponseBody) ? "" : $"[Response={entry.ResponseBody}] ")}" +
                $"{(string.IsNullOrWhiteSpace(entry.Error) ? "" : $"Error={entry.Error}")}";

            await File.AppendAllTextAsync(filePath, line + Environment.NewLine);
        }
    }
}
