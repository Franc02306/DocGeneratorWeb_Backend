using DocGenerator.Application.DTOs.Authentications;
using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.Helpers.Authentications;
using DocGenerator.Infrastructure.Repositories.Authentications;

namespace DocGenerator.Application.Services.Authentications
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthenticationService(IAuthenticationRepository authenticationRepository, JwtHelper jwtHelper)
        {
            _authenticationRepository = authenticationRepository;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        public async Task<ApiResponse<LoginResponse>> LoginUserAsync(LoginRequest request)
        {
            // 1. Validar los campos requeridos
            if (request == null)
                return ApiResponse<LoginResponse>.Fail("Request inválido.");

            if (string.IsNullOrWhiteSpace(request.UserName))
                return ApiResponse<LoginResponse>.Fail("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<LoginResponse>.Fail("La contraseña es obligatoria.");

            // 2. Obtener datos por el nombre de usuario y validar si cumple las condiciones
            var user = await _authenticationRepository.GetUserByUserNameAsync(request.UserName.Trim());

            if (user == null)
                return ApiResponse<LoginResponse>.Fail("Usuario o contraseña incorrectos.");

            if (!user.IsActive)
                return ApiResponse<LoginResponse>.Fail("El usuario se encuentra inactivo.");

            // 3. Verificar la contraseña del usuario con la del request
            var passwordValid = PasswordHelper.Verify(request.Password, user.Password);

            if (!passwordValid)
                return ApiResponse<LoginResponse>.Fail("Usuario o contraseña incorrectos.");

            // 4 Generar el token JWT
            var token = _jwtHelper.GenerateToken(user);

            var response = new LoginResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };

            // 5. Retornar respuesta
            return ApiResponse<LoginResponse>.Ok(response, "Inicio de sesión correcto.");
        }
    }
}
