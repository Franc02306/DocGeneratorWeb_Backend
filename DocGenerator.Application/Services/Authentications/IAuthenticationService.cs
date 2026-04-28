using DocGenerator.Application.DTOs.Authentications;
using DocGenerator.Application.DTOs.Commons;

namespace DocGenerator.Application.Services.Authentications
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        Task<ApiResponse<LoginResponse>> LoginUserAsync(LoginRequest request);
    }
}
