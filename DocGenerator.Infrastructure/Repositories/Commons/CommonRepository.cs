using DocGenerator.Infrastructure.Helpers;
using DocGenerator.Infrastructure.Persistence;

namespace DocGenerator.Infrastructure.Repositories.Commons
{
    public class CommonRepository : ICommonRepository
    {
        private readonly DbConnectionFactory _factory;

        public CommonRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Valida si la moneda existe en SAP.
        /// </summary>
        public async Task<bool> ExistsCurrencyInSapAsync(string currency)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Commons", "ExistsCurrencyInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, currency);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el código de retención existe en SAP.
        /// </summary>
        public async Task<bool> ExistsRetentionCodeInSapAsync(string retentionCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsRetentionCodeInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, retentionCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }
    }
}
