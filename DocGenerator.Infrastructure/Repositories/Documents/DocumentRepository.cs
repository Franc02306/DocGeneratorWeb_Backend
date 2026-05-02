using DocGenerator.Domain.Entities;
using DocGenerator.Infrastructure.Helpers;
using DocGenerator.Infrastructure.Persistence;
using System.Data;
using System.Data.Odbc;

namespace DocGenerator.Infrastructure.Repositories.Documents
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DbConnectionFactory _factory;

        public DocumentRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        #region DOCUMENTO | CABECERA

        /// <summary>
        /// Creación de la cabecera del documento
        /// </summary>
        public async Task<int> CreateDocumentAsync(Document document)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "CreateDocument.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, document.UserId);
            DbHelper.AddParameter(cmd, document.PostingDate);
            DbHelper.AddParameter(cmd, document.DocumentDate);
            DbHelper.AddParameter(cmd, document.DueDate);
            DbHelper.AddParameter(cmd, document.CardCode);
            DbHelper.AddParameter(cmd, document.Ruc);
            DbHelper.AddParameter(cmd, document.CardName);
            DbHelper.AddParameter(cmd, document.Currency);
            DbHelper.AddParameter(cmd, document.Comments);
            DbHelper.AddParameter(cmd, document.DocumentType);
            DbHelper.AddParameter(cmd, document.Series);
            DbHelper.AddParameter(cmd, document.Correlative);
            DbHelper.AddParameter(cmd, document.IsValidSunat.HasValue ? (document.IsValidSunat.Value ? 1 : 0) : (object?)null);
            DbHelper.AddParameter(cmd, document.RetentionTypeCode);
            DbHelper.AddParameter(cmd, document.RetentionCode);
            DbHelper.AddParameter(cmd, document.DocumentTotal);

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
        /// Eliminar documento por ID
        /// </summary>
        public async Task<int> DeleteDocumentByIdAsync(int documentId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "DeleteDocumentById.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, documentId);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Valida si ya existe un documento en la web con la misma clave.
        /// </summary>
        public async Task<bool> ExistsDocumentAsync(string ruc, string documentType, string series, string correlative)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDocument.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, ruc);
            DbHelper.AddParameter(cmd, documentType);
            DbHelper.AddParameter(cmd, series);
            DbHelper.AddParameter(cmd, correlative);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el documento ya existe en SAP
        /// </summary>
        public async Task<bool> ExistsDocumentInSapAsync(string ruc, string documentType, string series, string correlative)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDocumentInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, ruc);
            DbHelper.AddParameter(cmd, documentType);
            DbHelper.AddParameter(cmd, series);
            DbHelper.AddParameter(cmd, correlative);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el documento existe por ID
        /// </summary>
        public async Task<bool> ExistsDocumentByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDocumentById.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, id);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida duplicidad en WEB excluyendo el mismo documento
        /// </summary>
        public async Task<bool> ExistsDocumentExceptIdAsync(int id, string ruc, string documentType, string series, string correlative)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDocumentExceptId.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, ruc);
            DbHelper.AddParameter(cmd, documentType);
            DbHelper.AddParameter(cmd, series);
            DbHelper.AddParameter(cmd, correlative);
            DbHelper.AddParameter(cmd, id);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Actualiza la cabecera del documento
        /// </summary>
        public async Task<int> UpdateDocumentAsync(Document document)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "UpdateDocument.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, document.UserId);
            DbHelper.AddParameter(cmd, document.PostingDate);
            DbHelper.AddParameter(cmd, document.DocumentDate);
            DbHelper.AddParameter(cmd, document.DueDate);
            DbHelper.AddParameter(cmd, document.CardCode);
            DbHelper.AddParameter(cmd, document.Ruc);
            DbHelper.AddParameter(cmd, document.CardName);
            DbHelper.AddParameter(cmd, document.Currency);
            DbHelper.AddParameter(cmd, document.Comments);
            DbHelper.AddParameter(cmd, document.DocumentType);
            DbHelper.AddParameter(cmd, document.Series);
            DbHelper.AddParameter(cmd, document.Correlative);
            DbHelper.AddParameter(cmd, document.IsValidSunat.HasValue ? (document.IsValidSunat.Value ? 1 : 0) : (object?)null);
            DbHelper.AddParameter(cmd, document.RetentionTypeCode);
            DbHelper.AddParameter(cmd, document.RetentionCode);
            DbHelper.AddParameter(cmd, document.DocumentTotal);
            DbHelper.AddParameter(cmd, document.Id);

            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region DOCUMENTO | DETALLE

        /// <summary>
        /// Creación de los detalles del documento
        /// </summary>
        public async Task<int> CreateDocumentDetailsAsync(List<DocumentDetail> details)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "CreateDocumentDetail.sql");
            var sql = await File.ReadAllTextAsync(path);

            int rows = 0;

            foreach (var d in details)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                DbHelper.AddParameter(cmd, d.DocId);
                DbHelper.AddParameter(cmd, d.ItemCode);
                DbHelper.AddParameter(cmd, d.Quantity);
                DbHelper.AddParameter(cmd, d.Price);
                DbHelper.AddParameter(cmd, d.Subtotal);
                DbHelper.AddParameter(cmd, d.TaxCode);
                DbHelper.AddParameter(cmd, d.Total);
                DbHelper.AddParameter(cmd, d.ProjectCode);
                DbHelper.AddParameter(cmd, d.Dim1);
                DbHelper.AddParameter(cmd, d.Dim2);
                DbHelper.AddParameter(cmd, d.Dim3);
                DbHelper.AddParameter(cmd, d.Dim4);
                DbHelper.AddParameter(cmd, d.Dim5);
                DbHelper.AddParameter(cmd, d.WarehouseCode);

                rows += cmd.ExecuteNonQuery();
            }

            return rows;
        }

        /// <summary>
        /// Valida si el artículo existe en SAP
        /// </summary>
        public async Task<bool> ExistsItemInSapAsync(string itemCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsItemInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, itemCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el almacén existe en SAP
        /// </summary>
        public async Task<bool> ExistsWarehouseInSapAsync(string warehouseCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsWarehouseInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, warehouseCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el código de impuesto existe en SAP
        /// </summary>
        public async Task<bool> ExistsTaxCodeInSapAsync(string taxCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsTaxCodeInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, taxCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si el proyecto existe en SAP
        /// </summary>
        public async Task<bool> ExistsProjectInSapAsync(string projectCode)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsProjectInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, projectCode);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Valida si una dimensión existe en SAP según su código de dimensión
        /// </summary>
        public async Task<bool> ExistsDimensionInSapAsync(int dimensionCode, string dimensionValue)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDimensionInSap.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, dimensionCode);
            DbHelper.AddParameter(cmd, dimensionValue);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Actualiza un detalle de documento existente
        /// </summary>
        public async Task<int> UpdateDocumentDetailAsync(DocumentDetail detail)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "UpdateDocumentDetail.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, detail.ItemCode);
            DbHelper.AddParameter(cmd, detail.Quantity);
            DbHelper.AddParameter(cmd, detail.Price);
            DbHelper.AddParameter(cmd, detail.Subtotal);
            DbHelper.AddParameter(cmd, detail.TaxCode);
            DbHelper.AddParameter(cmd, detail.Total);
            DbHelper.AddParameter(cmd, detail.ProjectCode);
            DbHelper.AddParameter(cmd, detail.Dim1);
            DbHelper.AddParameter(cmd, detail.Dim2);
            DbHelper.AddParameter(cmd, detail.Dim3);
            DbHelper.AddParameter(cmd, detail.Dim4);
            DbHelper.AddParameter(cmd, detail.Dim5);
            DbHelper.AddParameter(cmd, detail.WarehouseCode);
            DbHelper.AddParameter(cmd, detail.Id);
            DbHelper.AddParameter(cmd, detail.DocId);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Elimina un detalle específico de un documento
        /// </summary>
        public async Task<int> DeleteDocumentDetailByIdAsync(int detailId, int documentId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "DeleteDocumentDetailById.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, detailId);
            DbHelper.AddParameter(cmd, documentId);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Valida si un detalle pertenece a un documento
        /// </summary>
        public async Task<bool> ExistsDocumentDetailByIdAsync(int detailId, int documentId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();

            var path = DbHelper.GetQueryPath(_factory.GetProvider(), "Documents", "ExistsDocumentDetailById.sql");
            var sql = await File.ReadAllTextAsync(path);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DbHelper.AddParameter(cmd, detailId);
            DbHelper.AddParameter(cmd, documentId);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result) > 0;
        }

        #endregion
    }
}
