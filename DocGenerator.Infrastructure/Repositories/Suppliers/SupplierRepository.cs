using DocGenerator.Infrastructure.Helpers;
using DocGenerator.Infrastructure.Persistence;

namespace DocGenerator.Infrastructure.Repositories.Suppliers
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DbConnectionFactory _factory;

        public SupplierRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Valida si el proveedor existe en SAP
        /// </summary>
        public async Task<bool> ExistsSupplierInSapAsync(string cardCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Suppliers", "ExistsSupplierInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, cardCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida que el CardCode corresponda al RUC en SAP
        /// </summary>
        public async Task<bool> IsSupplierRucMatchAsync(string cardCode, string ruc)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Suppliers", "IsSupplierRucMatch.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, cardCode);
            DbHelper.AddParameter(cmd, ruc);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el proveedor tiene habilitado el código de retención en SAP
        /// </summary>
        public async Task<bool> SupplierHasRetentionCodeAsync(string supplierCode, string retentionCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Suppliers", "SupplierHasRetentionCode.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, supplierCode);
            DbHelper.AddParameter(cmd, retentionCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }
    }
}
