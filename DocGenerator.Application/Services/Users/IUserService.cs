using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.DTOs.Users;

namespace DocGenerator.Application.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<ApiResponse<int>> CreateUserAsync(CreateUserRequest request);

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        Task<ApiResponse<int>> UpdateUserAsync(UpdateUserRequest request);
    }
}
