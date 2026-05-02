using DocGenerator.Domain.Entities;

namespace DocGenerator.Infrastructure.Repositories.Documents
{
    public interface IDocumentRepository
    {
        #region DOCUMENTO | CABECERA

        /// <summary>
        /// Creación de la cabecera del documento
        /// </summary>
        Task<int> CreateDocumentAsync(Document document);

        /// <summary>
        /// Eliminar documento por ID
        /// </summary>
        Task<int> DeleteDocumentByIdAsync(int documentId);

        /// <summary>
        /// Valida si ya existe un documento en la web con la misma clave
        /// </summary>
        Task<bool> ExistsDocumentAsync(string ruc, string documentType, string series, string correlative);

        /// <summary>
        /// Valida si el documento ya existe en SAP
        /// </summary>
        Task<bool> ExistsDocumentInSapAsync(string ruc, string documentType, string series, string correlative);

        /// <summary>
        /// Valida si el documento existe por ID
        /// </summary>
        Task<bool> ExistsDocumentByIdAsync(int id);

        /// <summary>
        /// Valida duplicidad en WEB excluyendo el mismo documento
        /// </summary>
        Task<bool> ExistsDocumentExceptIdAsync(int id, string ruc, string documentType, string series, string correlative);

        /// <summary>
        /// Actualiza la cabecera del documento
        /// </summary>
        Task<int> UpdateDocumentAsync(Document document);

        #endregion

        #region DOCUMENTO | DETALLE

        /// <summary>
        /// Creación de los detalles del documento
        /// </summary>
        Task<int> CreateDocumentDetailsAsync(List<DocumentDetail> details);

        /// <summary>
        /// Valida si el artículo existe en SAP
        /// </summary>
        Task<bool> ExistsItemInSapAsync(string itemCode);

        /// <summary>
        /// Valida si el almacén existe en SAP
        /// </summary>
        Task<bool> ExistsWarehouseInSapAsync(string warehouseCode);

        /// <summary>
        /// Valida si el código de impuesto existe en SAP
        /// </summary>
        Task<bool> ExistsTaxCodeInSapAsync(string taxCode);

        /// <summary>
        /// Valida si el proyecto existe en SAP
        /// </summary>
        Task<bool> ExistsProjectInSapAsync(string projectCode);

        /// <summary>
        /// Valida si una dimensión existe en SAP según su código de dimensión
        /// </summary>
        Task<bool> ExistsDimensionInSapAsync(int dimensionCode, string dimensionValue);

        /// <summary>
        /// Actualiza un detalle de documento existente
        /// </summary>
        Task<int> UpdateDocumentDetailAsync(DocumentDetail detail);

        /// <summary>
        /// Elimina un detalle específico de un documento
        /// </summary>
        Task<int> DeleteDocumentDetailByIdAsync(int detailId, int documentId);

        /// <summary>
        /// Valida si un detalle pertenece a un documento
        /// </summary>
        Task<bool> ExistsDocumentDetailByIdAsync(int detailId, int documentId);

        #endregion
    }
}
