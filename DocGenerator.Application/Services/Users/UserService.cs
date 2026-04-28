using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.DTOs.Users;
using DocGenerator.Application.Helpers.Authentications;
using DocGenerator.Application.Helpers.Users;
using DocGenerator.Application.Services.EmailNotifications;
using DocGenerator.Domain.Entities;
using DocGenerator.Infrastructure.Repositories.Users;

namespace DocGenerator.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailNotification _emailNotification;
        public UserService(IUserRepository userRepository, IEmailNotification emailNotification)
        {
            _userRepository = userRepository;
            _emailNotification = emailNotification;
        }

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        public async Task<ApiResponse<int>> CreateAsync(CreateUserRequest request)
        {
            // 1. Validaciones
            var errors = UserValidatorHelper.ValidateCreate(request);
            errors.AddRange(PasswordHelper.Validate(request.Password));

            if (!errors.Any())
            {
                if (await _userRepository.ExistsUserNameAsync(request.UserName.Trim()))
                    errors.Add("El nombre de usuario ya se encuentra registrado.");

                if (await _userRepository.ExistsEmailAsync(request.Email.Trim()))
                    errors.Add("El correo electrónico ya se encuentra registrado.");
            }

            if (errors.Any())
                return ApiResponse<int>.Fail(errors);

            // 2. Preparar entidad
            var user = new User
            {
                UserName = request.UserName.Trim(),
                Email = request.Email.Trim(),
                Password = PasswordHelper.Hash(request.Password),
                CreatedAt = DateTime.Now
            };

            // 3. Crear usuario (debe devolver ID)
            var userId = await _userRepository.CreateUserAsync(user);

            // 4. Envío de correo + rollback si falla
            if (userId > 0)
            {
                try
                {
                    await _emailNotification.SendUserVerificationToken(user.UserName, user.Email);
                }
                catch (Exception)
                {
                    await _userRepository.DeleteUserByIdAsync(userId);

                    return ApiResponse<int>.Fail("No se pudo enviar el correo. Se revirtió la creación del usuario.");
                }

                return ApiResponse<int>.Ok(userId, "Usuario creado correctamente.");
            }

            return ApiResponse<int>.Fail("No se pudo crear el usuario.");
        }
    }
}
