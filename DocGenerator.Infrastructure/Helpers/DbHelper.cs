using System.Data;

namespace DocGenerator.Infrastructure.Helpers
{
    public static class DbHelper
    {
        /// <summary>
        /// Agrega un parámetro al comando de base de datos
        /// </summary>
        public static void AddParameter(IDbCommand cmd, object value)
        {
            var p = cmd.CreateParameter();
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        /// <summary>
        /// Construye y valida la ruta física de un archivo SQL según el proveedor y módulo
        /// </summary>
        public static string GetQueryPath(string provider, string module, string fileName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Queries", provider.Trim().ToUpper(), module, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el archivo SQL: {path}");

            return path;
        }
    }
}
