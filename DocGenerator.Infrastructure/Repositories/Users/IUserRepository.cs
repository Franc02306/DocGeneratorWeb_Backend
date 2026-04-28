using DocGenerator.Domain.Entities;

namespace DocGenerator.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<int> CreateUserAsync(User user);

        /// <summary>
        /// Valida si existe un usuario con el mismo username
        /// </summary>
        Task<bool> ExistsUserNameAsync(string userName);

        /// <summary>
        /// Valida si existe un usuario con el mismo email
        /// </summary>
        Task<bool> ExistsEmailAsync(string email);

        /// <summary>
        /// Eliminar al usuario por ID
        /// </summary>
        Task<int> DeleteUserByIdAsync(int id);

        /// <summary>
        /// Actualizar usuario en la web
        /// </summary>
        Task<int> UpdateUserAsync(User user);

        /// <summary>
        /// Valida si existe otro usuario con el mismo username
        /// </summary>
        Task<bool> ExistsUserNameExceptIdAsync(string userName, int id);

        /// <summary>
        /// Valida si existe otro usuario con el mismo email
        /// </summary>
        Task<bool> ExistsEmailExceptIdAsync(string email, int id);
    }
}
