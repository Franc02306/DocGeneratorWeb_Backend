using DocGenerator.Domain.Entities;

namespace DocGenerator.Infrastructure.Repositories.Authentications
{
    public interface IAuthenticationRepository
    {
        /// <summary>
        /// Obtiene usuario por username
        /// </summary>
        Task<User?> GetUserByUserNameAsync(string userName);
    }
}
