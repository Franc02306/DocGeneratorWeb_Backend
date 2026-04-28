using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Odbc;

namespace DocGenerator.Infrastructure.Persistence
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string provider = _configuration["DatabaseWeb:Provider"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(provider))
                throw new Exception("No se configuró DatabaseWeb:Provider.");

            provider = provider.Trim().ToUpper();

            return provider switch
            {
                "HANA" => CreateOdbcConnection("DatabaseWeb:ConnectionStringHana"),
                "SQLSERVER" => CreateOdbcConnection("DatabaseWeb:ConnectionStringSsms"),
                _ => throw new Exception($"Proveedor no válido: {provider}. Use HANA o SQLSERVER.")
            };
        }

        private IDbConnection CreateOdbcConnection(string connectionKey)
        {
            string connectionString = _configuration[connectionKey] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"No se configuró {connectionKey}.");

            return new OdbcConnection(connectionString);
        }

        public string GetProvider()
        {
            return (_configuration["DatabaseWeb:Provider"] ?? string.Empty)
                .Trim()
                .ToUpper();
        }
    }
}