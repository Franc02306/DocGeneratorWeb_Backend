using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.DTOs.Users;

namespace DocGenerator.Application.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<ApiResponse<int>> CreateAsync(CreateUserRequest request);
    }
}
