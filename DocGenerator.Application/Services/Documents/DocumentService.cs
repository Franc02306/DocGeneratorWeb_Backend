using DocGenerator.Application.DTOs.Commons;
using DocGenerator.Application.DTOs.Documents;
using DocGenerator.Application.Helpers.Documents;
using DocGenerator.Domain.Entities;
using DocGenerator.Infrastructure.Repositories.Commons;
using DocGenerator.Infrastructure.Repositories.Documents;
using DocGenerator.Infrastructure.Repositories.Suppliers;
using DocGenerator.Infrastructure.Repositories.Users;

namespace DocGenerator.Application.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICommonRepository _commonRepository;

        public DocumentService(IDocumentRepository documentRepository, IUserRepository userRepository, 
            ISupplierRepository supplierRepository, ICommonRepository commonRepository)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _supplierRepository = supplierRepository;
            _commonRepository = commonRepository;
        }

        /// <summary>
        /// Creación de un documento.
        /// Valida datos básicos, reglas de negocio y registra la cabecera del documento.
        /// </summary>
        public async Task<ApiResponse<int>> CreateDocumentAsync(CreateDocumentRequest request)
        {
            // 1. Validaciones
            var errors = DocumentValidator.ValidateCreate(request);
            errors.AddRange(DocumentDetailValidator.ValidateCreate(request.Details));

            if (errors.Any())
                return ApiResponse<int>.Fail(errors);

            errors.AddRange(await ValidateBusinessRulesAsync(request));
            errors.AddRange(await ValidateDetailBusinessRulesAsync(request.Details!));

            if (errors.Any())
                return ApiResponse<int>.Fail(errors);

            // 2. Crear cabecera
            var document = new Document
            {
                UserId = request.UserId,
                PostingDate = request.PostingDate,
                DocumentDate = request.DocumentDate,
                DueDate = request.DueDate,
                CardCode = request.CardCode,
                Ruc = request.Ruc,
                CardName = request.CardName,
                Currency = request.Currency,
                Comments = request.Comments,
                DocumentType = request.DocumentType,
                Series = request.Series,
                Correlative = request.Correlative,
                IsValidSunat = request.IsValidSunat,
                RetentionTypeCode = request.RetentionTypeCode,
                RetentionCode = request.RetentionCode,
                DocumentTotal = request.DocumentTotal,
                CreatedAt = DateTime.Now
            };

            var documentId = await _documentRepository.CreateDocumentAsync(document);

            if (documentId <= 0)
                return ApiResponse<int>.Fail("No se pudo crear el documento.");

            // 3. Crear detalles
            var details = request.Details!.Select(x => new DocumentDetail
            {
                DocId = documentId,
                ItemCode = x.ItemCode!,
                Quantity = x.Quantity!.Value,
                Price = x.Price!.Value,
                Subtotal = x.Subtotal!.Value,
                TaxCode = x.TaxCode,
                Total = x.Total!.Value,
                ProjectCode = x.ProjectCode,
                Dim1 = x.Dim1,
                Dim2 = x.Dim2,
                Dim3 = x.Dim3,
                Dim4 = x.Dim4,
                Dim5 = x.Dim5,
                WarehouseCode = x.WarehouseCode,
                CreatedAt = DateTime.Now
            }).ToList();

            var detailRows = await _documentRepository.CreateDocumentDetailsAsync(details);

            if (detailRows <= 0)
            {
                await _documentRepository.DeleteDocumentByIdAsync(documentId);

                return ApiResponse<int>.Fail("No se pudo crear el detalle del documento. Se revirtió la creación de la cabecera.");
            }

            // 4. Retornar respuesta
            return ApiResponse<int>.Ok(documentId, "Documento creado correctamente.");
        }

        /// <summary>
        /// Actualización de un documento.
        /// Valida datos básicos, reglas de negocio y registra la cabecera del documento.
        /// </summary>
        public async Task<ApiResponse<int>> UpdateDocumentAsync(int id, UpdateDocumentRequest request)
        {
            // 1. Validaciones
            if (id <= 0)
                return ApiResponse<int>.Fail("El ID del documento es obligatorio.");

            var errors = DocumentValidator.ValidateUpdate(request);
            errors.AddRange(DocumentDetailValidator.ValidateUpdate(request.Details));

            if (errors.Any())
                return ApiResponse<int>.Fail(errors);

            // 2. Validar existencia del documento
            if (!await _documentRepository.ExistsDocumentByIdAsync(id))
                return ApiResponse<int>.Fail("El documento no existe.");

            // 3. Validaciones de negocio (BD / SAP)
            errors.AddRange(await ValidateBusinessRulesUpdateAsync(id, request));
            errors.AddRange(await ValidateDetailBusinessRulesUpdateAsync(request.Details!));

            // Validaciones adicionales de detalles
            foreach (var detail in request.Details!)
            {
                if (detail.Id == 0 && detail.IsDeleted)
                    errors.Add("No se puede eliminar un detalle que aún no existe.");

                if (detail.Id > 0 &&
                    !await _documentRepository.ExistsDocumentDetailByIdAsync(detail.Id, id))
                    errors.Add($"El detalle con ID {detail.Id} no pertenece al documento.");
            }

            if (errors.Any())
                return ApiResponse<int>.Fail(errors);

            // 4. Actualizar cabecera
            var document = new Document
            {
                Id = id,
                PostingDate = request.PostingDate,
                DocumentDate = request.DocumentDate,
                DueDate = request.DueDate,
                CardCode = request.CardCode,
                Ruc = request.Ruc,
                CardName = request.CardName,
                Currency = request.Currency,
                Comments = request.Comments,
                DocumentType = request.DocumentType,
                Series = request.Series,
                Correlative = request.Correlative,
                IsValidSunat = request.IsValidSunat,
                RetentionTypeCode = request.RetentionTypeCode,
                RetentionCode = request.RetentionCode,
                DocumentTotal = request.DocumentTotal,
                UpdatedAt = DateTime.Now
            };

            var rows = await _documentRepository.UpdateDocumentAsync(document);

            if (rows <= 0)
                return ApiResponse<int>.Fail("No se pudo actualizar el documento.");

            // 5. Procesar detalles (Crear - Actualizar - Eliminar)
            foreach (var item in request.Details!)
            {
                // Eliminar detalle
                if (item.IsDeleted)
                {
                    if (item.Id > 0)
                        await _documentRepository.DeleteDocumentDetailByIdAsync(item.Id, id);

                    continue;
                }

                var detail = new DocumentDetail
                {
                    Id = item.Id,
                    DocId = id,
                    ItemCode = item.ItemCode!,
                    Quantity = item.Quantity!.Value,
                    Price = item.Price!.Value,
                    Subtotal = item.Subtotal!.Value,
                    TaxCode = item.TaxCode,
                    Total = item.Total!.Value,
                    ProjectCode = item.ProjectCode,
                    Dim1 = item.Dim1,
                    Dim2 = item.Dim2,
                    Dim3 = item.Dim3,
                    Dim4 = item.Dim4,
                    Dim5 = item.Dim5,
                    WarehouseCode = item.WarehouseCode,
                    UpdatedAt = DateTime.Now
                };

                // Crear detalle
                if (item.Id == 0)
                {
                    await _documentRepository.CreateDocumentDetailsAsync(new List<DocumentDetail> { detail });
                }
                // Actualizar detalle
                else
                {
                    await _documentRepository.UpdateDocumentDetailAsync(detail);
                }
            }

            // 6. Retornar respuesta
            return ApiResponse<int>.Ok(id, "Documento actualizado correctamente.");
        }

        /// <summary>
        /// Ejecuta validaciones contra BD web y SAP para crear el documento
        /// </summary>
        private async Task<List<string>> ValidateBusinessRulesAsync(CreateDocumentRequest request)
        {
            var errors = new List<string>();

            if (!await _userRepository.ExistsActiveUserAsync(request.UserId))
                errors.Add("El usuario no existe o no se encuentra activo.");

            if (await _documentRepository.ExistsDocumentAsync(
                    request.Ruc!,
                    request.DocumentType!,
                    request.Series!,
                    request.Correlative!))
                errors.Add("El documento ya se encuentra registrado en la web.");

            if (!await _supplierRepository.ExistsSupplierInSapAsync(request.CardCode!))
                errors.Add("El proveedor no existe en SAP.");

            if (!await _supplierRepository.IsSupplierRucMatchAsync(request.CardCode!, request.Ruc!))
                errors.Add("El RUC no corresponde al proveedor indicado en SAP.");

            if (!await _commonRepository.ExistsCurrencyInSapAsync(request.Currency!))
                errors.Add("La moneda no existe en SAP.");

            if (!string.IsNullOrWhiteSpace(request.RetentionCode))
            {
                if (!await _commonRepository.ExistsRetentionCodeInSapAsync(request.RetentionCode))
                    errors.Add("El código de retención no existe en SAP.");

                if (!await _supplierRepository.SupplierHasRetentionCodeAsync(request.CardCode!, request.RetentionCode))
                    errors.Add("El proveedor no tiene habilitado el código de retención indicado.");
            }

            if (await _documentRepository.ExistsDocumentInSapAsync(
                    request.Ruc!,
                    request.DocumentType!,
                    request.Series!,
                    request.Correlative!))
                errors.Add("El documento ya se encuentra registrado en SAP.");

            return errors;
        }

        /// <summary>
        /// Valida reglas de negocio del detalle contra SAP
        /// </summary>
        private async Task<List<string>> ValidateDetailBusinessRulesAsync(List<CreateDocDetailRequest> details)
        {
            var errors = new List<string>();

            for (int i = 0; i < details.Count; i++)
            {
                var line = details[i];
                var lineNumber = i + 1;

                // ITEM
                if (!await _documentRepository.ExistsItemInSapAsync(line.ItemCode!))
                    errors.Add($"Línea {lineNumber}: El artículo no existe en SAP.");

                // WAREHOUSE
                if (!string.IsNullOrWhiteSpace(line.WarehouseCode) &&
                    !await _documentRepository.ExistsWarehouseInSapAsync(line.WarehouseCode))
                    errors.Add($"Línea {lineNumber}: El almacén no existe en SAP.");

                // TAX
                if (!string.IsNullOrWhiteSpace(line.TaxCode) &&
                    !await _documentRepository.ExistsTaxCodeInSapAsync(line.TaxCode))
                    errors.Add($"Línea {lineNumber}: El impuesto no existe en SAP.");

                // PROJECT
                if (!string.IsNullOrWhiteSpace(line.ProjectCode) &&
                    !await _documentRepository.ExistsProjectInSapAsync(line.ProjectCode))
                    errors.Add($"Línea {lineNumber}: El proyecto no existe en SAP.");

                // DIMENSIONES
                if (!string.IsNullOrWhiteSpace(line.Dim1) &&
                    !await _documentRepository.ExistsDimensionInSapAsync(1, line.Dim1))
                    errors.Add($"Línea {lineNumber}: Dimensión 1 no existe en SAP.");

                if (!string.IsNullOrWhiteSpace(line.Dim2) &&
                    !await _documentRepository.ExistsDimensionInSapAsync(2, line.Dim2))
                    errors.Add($"Línea {lineNumber}: Dimensión 2 no existe en SAP.");

                if (!string.IsNullOrWhiteSpace(line.Dim3) &&
                    !await _documentRepository.ExistsDimensionInSapAsync(3, line.Dim3))
                    errors.Add($"Línea {lineNumber}: Dimensión 3 no existe en SAP.");

                if (!string.IsNullOrWhiteSpace(line.Dim4) &&
                    !await _documentRepository.ExistsDimensionInSapAsync(4, line.Dim4))
                    errors.Add($"Línea {lineNumber}: Dimensión 4 no existe en SAP.");

                if (!string.IsNullOrWhiteSpace(line.Dim5) &&
                    !await _documentRepository.ExistsDimensionInSapAsync(5, line.Dim5))
                    errors.Add($"Línea {lineNumber}: Dimensión 5 no existe en SAP.");
            }

            return errors;
        }

        /// <summary>
        /// Ejecuta validaciones contra BD web y SAP para actualizar el documento
        /// </summary>
        private async Task<List<string>> ValidateBusinessRulesUpdateAsync(int id, UpdateDocumentRequest request)
        {
            var errors = new List<string>();

            if (await _documentRepository.ExistsDocumentExceptIdAsync(
                    id,
                    request.Ruc!,
                    request.DocumentType!,
                    request.Series!,
                    request.Correlative!))
                errors.Add("El documento ya se encuentra registrado en la web.");

            if (!await _supplierRepository.ExistsSupplierInSapAsync(request.CardCode!))
                errors.Add("El proveedor no existe en SAP.");

            if (!await _supplierRepository.IsSupplierRucMatchAsync(request.CardCode!, request.Ruc!))
                errors.Add("El RUC no corresponde al proveedor.");

            if (!await _commonRepository.ExistsCurrencyInSapAsync(request.Currency!))
                errors.Add("La moneda no existe en SAP.");

            if (!string.IsNullOrWhiteSpace(request.RetentionCode))
            {
                if (!await _commonRepository.ExistsRetentionCodeInSapAsync(request.RetentionCode))
                    errors.Add("El código de retención no existe en SAP.");

                if (!await _supplierRepository.SupplierHasRetentionCodeAsync(request.CardCode!, request.RetentionCode))
                    errors.Add("El proveedor no tiene habilitado el código de retención.");
            }

            if (await _documentRepository.ExistsDocumentInSapAsync(
                    request.Ruc!,
                    request.DocumentType!,
                    request.Series!,
                    request.Correlative!))
                errors.Add("El documento ya se encuentra registrado en SAP.");

            return errors;
        }

        /// <summary>
        /// Valida reglas de negocio del detalle contra SAP para actualizar el documento
        /// Solo valida líneas que no están marcadas como eliminadas
        /// </summary>
        private async Task<List<string>> ValidateDetailBusinessRulesUpdateAsync(List<UpdateDocDetailRequest> details)
        {
            var activeDetails = details
                .Where(x => !x.IsDeleted)
                .Select(x => new CreateDocDetailRequest
                {
                    ItemCode = x.ItemCode,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Subtotal = x.Subtotal,
                    TaxCode = x.TaxCode,
                    Total = x.Total,
                    ProjectCode = x.ProjectCode,
                    Dim1 = x.Dim1,
                    Dim2 = x.Dim2,
                    Dim3 = x.Dim3,
                    Dim4 = x.Dim4,
                    Dim5 = x.Dim5,
                    WarehouseCode = x.WarehouseCode
                })
                .ToList();

            return await ValidateDetailBusinessRulesAsync(activeDetails);
        }
    }
}
