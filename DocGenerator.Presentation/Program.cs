using DocGenerator.Application.Helpers.Authentications;
using DocGenerator.Application.Services.Authentications;
using DocGenerator.Application.Services.EmailNotifications;
using DocGenerator.Application.Services.Logs;
using DocGenerator.Application.Services.Users;
using DocGenerator.Application.Settings;
using DocGenerator.Infrastructure.Persistence;
using DocGenerator.Infrastructure.Repositories.Authentications;
using DocGenerator.Infrastructure.Repositories.Documents;
using DocGenerator.Infrastructure.Repositories.Users;
using DocGenerator.Presentation.Middlewares;

namespace DocGenerator.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Inyección de dependencias
            builder.Services.AddScoped<DbConnectionFactory>();

            // Repositorios
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Servicios
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<IEmailNotification, EmailNotification>();

            // Helpers
            builder.Services.AddScoped<JwtHelper>();

            // Configuraciones
            builder.Services.Configure<DocGeneratorWebSettings>(
                builder.Configuration.GetSection("DocGeneratorWebSettings"));

            builder.Services.Configure<SunatEndpointSettings>(
                builder.Configuration.GetSection("SunatEndpointSettings"));

            builder.Services.Configure<SmtpSettings>(
                builder.Configuration.GetSection("SmtpSettings"));

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            var app = builder.Build();

            // Middlewares
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
