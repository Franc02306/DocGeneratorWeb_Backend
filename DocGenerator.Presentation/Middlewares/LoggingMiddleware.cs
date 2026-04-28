using DocGenerator.Application.Services.Logs;
using DocGenerator.Domain.Enums;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace DocGenerator.Presentation.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepta request y response y registra la información junto con errores reales.
        /// </summary>
        public async Task InvokeAsync(HttpContext context, ILogService logService)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            var date = DateTime.Now;
            context.Request.EnableBuffering();

            string? requestBody;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            requestBody = Hide(requestBody);

            var originalBody = context.Response.Body;
            using var mem = new MemoryStream();
            context.Response.Body = mem;

            await _next(context);

            mem.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(mem).ReadToEndAsync();
            mem.Seek(0, SeekOrigin.Begin);

            responseText = Hide(responseText);

            var realError = context.Items.ContainsKey("RealException")
                ? context.Items["RealException"]?.ToString()
                : null;

            await logService.WriteAsync(new LogEntry
            {
                Date = date,
                Module = ResolveModule(context),
                Action = ResolveAction(context),
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                User = ResolveUser(context),
                RequestBody = string.IsNullOrWhiteSpace(requestBody) ? null : requestBody,
                ResponseBody = string.IsNullOrWhiteSpace(responseText) ? null : responseText,
                Error = realError
            });

            await mem.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }

        private static string ResolveModule(HttpContext c) =>
            c.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) is var s && s?.Length >= 2 ? s[1] : "General";

        private static string ResolveAction(HttpContext c) =>
            c.GetEndpoint()?.DisplayName ?? "UnknownAction";

        private static string? ResolveUser(HttpContext c) =>
            c.User?.FindFirst(ClaimTypes.Name)?.Value;

        private static string? Hide(string? b) =>
            string.IsNullOrWhiteSpace(b)
                ? b
                : Regex.Replace(b, "\"(password|currentPassword|newPassword|confirmPassword|token|accessToken|refreshToken|clientSecret|client_secret|secret)\"\\s*:\\s*\".*?\"", m => $"\"{m.Groups[1].Value}\":\"***\"", RegexOptions.IgnoreCase);
    }
}