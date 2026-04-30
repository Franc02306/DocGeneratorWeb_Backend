using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.DTOs.Documents;

namespace DocGenerator.Application.Services.Documents
{
    public interface IDocumentService
    {
        /// <summary>
        /// Creación de un documento (cabecera + detalle)
        /// </summary>
        Task<ApiResponse<int>> CreateDocumentAsync(CreateDocumentRequest request);
    }
}
