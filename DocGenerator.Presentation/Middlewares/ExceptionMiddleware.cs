using System.Net;
using System.Text.Json;
using System.Text.Encodings.Web;
using DocGenerator.Application.DTOs.Commons;

namespace DocGenerator.Presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Captura excepciones no controladas y guarda el error real en el contexto.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Items["RealException"] = ex.ToString();
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Devuelve una respuesta estándar al cliente.
        /// </summary>
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = ApiResponse<string>.Fail(
                context.Response.StatusCode == 500
                    ? "Ocurrió un error interno en el servidor."
                    : ex.Message
            );

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            await context.Response.WriteAsync(json);
        }
    }
}