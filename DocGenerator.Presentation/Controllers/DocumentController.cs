using DocGenerator.Application.DTOs.Documents;
using DocGenerator.Application.Services.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocGenerator.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        #region POST

        /// <summary>
        /// Creación de un documento (cabecera + detalle)
        /// </summary>
        [HttpPost("create-document")]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
        {
            var response = await _documentService.CreateDocumentAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        #endregion
    }
}
