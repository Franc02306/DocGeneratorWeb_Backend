using DocGenerator.Domain.Entities;
using DocGenerator.Infrastructure.Helpers;
using DocGenerator.Infrastructure.Persistence;

namespace DocGenerator.Infrastructure.Repositories.Authentications
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DbConnectionFactory _factory;

        public AuthenticationRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Obtiene usuario por username
        /// </summary>
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Authentications", "GetUserByUserName.sql");

            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, userName);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new User
            {
                Id = Convert.ToInt32(reader["ID"]),
                UserName = reader["USERNAME"]?.ToString() ?? string.Empty,
                Email = reader["EMAIL"]?.ToString() ?? string.Empty,
                Password = reader["PASSWORD"]?.ToString() ?? string.Empty,
                IsActive = Convert.ToInt32(reader["IS_ACTIVE"]) == 1,
                CreatedAt = Convert.ToDateTime(reader["CREATED_AT"])
            };
        }
    }
}
