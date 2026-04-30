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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

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

            // Configuraciones
            builder.Services.Configure<DocGeneratorWebSettings>(
                builder.Configuration.GetSection("DocGeneratorWebSettings"));

            builder.Services.Configure<SunatEndpointSettings>(
                builder.Configuration.GetSection("SunatEndpointSettings"));

            builder.Services.Configure<SmtpSettings>(
                builder.Configuration.GetSection("SmtpSettings"));

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            success = false,
                            message = "No autorizado. Debe de iniciar sesión.",
                            data = (object?)null,
                            errors = (object?)null
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            success = false,
                            message = "Acceso denegado. No tiene permisos para acceder a este recurso.",
                            data = (object?)null,
                            errors = (object?)null
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                };
            });

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}