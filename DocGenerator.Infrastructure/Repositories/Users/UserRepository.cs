using DocGenerator.Domain.Entities;
using DocGenerator.Infrastructure.Helpers;
using DocGenerator.Infrastructure.Persistence;
using System.Data;
using System.Data.Odbc;

namespace DocGenerator.Infrastructure.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _factory;

        public UserRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Users", "CreateUser.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, user.UserName);
            DbHelper.AddParameter(cmd, user.Email);
            DbHelper.AddParameter(cmd, user.Password);
            DbHelper.AddParameter(cmd, user.IsActive ? 1 : 0);

            var outputId = new OdbcParameter
            {
                Direction = ParameterDirection.Output,
                OdbcType = OdbcType.Int
            };

            cmd.Parameters.Add(outputId);

            cmd.ExecuteNonQuery();

            return Convert.ToInt32(outputId.Value);
        }

        /// <summary>
        /// Valida si existe un usuario con el mismo username
        /// </summary>
        public async Task<bool> ExistsUserNameAsync(string userName)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Users", "ExistsUserName.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, userName);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si existe un usuario con el mismo email
        /// </summary>
        public async Task<bool> ExistsEmailAsync(string email)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Users", "ExistsEmail.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, email);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Eliminar al usuario por ID
        /// </summary>
        public async Task<int> DeleteUserByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Users", "DeleteUserById.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, id);

            return cmd.ExecuteNonQuery();
        }
    }
}
