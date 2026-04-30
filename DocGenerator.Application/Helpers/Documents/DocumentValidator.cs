using DocGenerator.Application.DTOs.Documents;

namespace DocGenerator.Application.Helpers.Documents
{
    public static class DocumentValidator
    {
        /// <summary>
        /// Ejecuta todas las validaciones necesarias para la creación de un documento.
        /// </summary>
        public static List<string> ValidateCreate(CreateDocumentRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("La solicitud del documento es obligatoria.");
                return errors;
            }

            Normalize(request);

            ValidateRequiredFields(request, errors);
            ValidateMaxLengths(request, errors);
            ValidateFormats(request, errors);
            ValidateRetention(request, errors);

            // SUNAT:
            // Validación pendiente en otro servicio.

            return errors;
        }

        /// <summary>
        /// Normaliza los valores del request (trim, mayúsculas, limpieza de datos).
        /// </summary>
        private static void Normalize(CreateDocumentRequest request)
        {
            request.CardCode = request.CardCode?.Trim();
            request.Ruc = request.Ruc?.Trim();
            request.CardName = request.CardName?.Trim();
            request.Currency = request.Currency?.Trim().ToUpper();
            request.Comments = request.Comments?.Trim();
            request.DocumentType = request.DocumentType?.Trim();
            request.Series = request.Series?.Trim().ToUpper();
            request.Correlative = request.Correlative?.Trim();
            request.RetentionTypeCode = request.RetentionTypeCode?.Trim();
            request.RetentionCode = request.RetentionCode?.Trim();

            if (!string.IsNullOrWhiteSpace(request.Ruc))
                request.Ruc = new string(request.Ruc.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrWhiteSpace(request.Correlative))
                request.Correlative = request.Correlative.Replace(" ", "");
        }

        /// <summary>
        /// Valida campos obligatorios del documento.
        /// </summary>
        private static void ValidateRequiredFields(CreateDocumentRequest request, List<string> errors)
        {
            if (request.UserId <= 0)
                errors.Add("El usuario es obligatorio.");

            if (!request.PostingDate.HasValue)
                errors.Add("La fecha de contabilización es obligatoria.");

            if (!request.DocumentDate.HasValue)
                errors.Add("La fecha del documento es obligatoria.");

            if (!request.DueDate.HasValue)
                errors.Add("La fecha de vencimiento es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.CardCode))
                errors.Add("El código del proveedor es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Ruc))
                errors.Add("El RUC es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.CardName))
                errors.Add("La razón social es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.DocumentType))
                errors.Add("El tipo de documento es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Series))
                errors.Add("La serie es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.Correlative))
                errors.Add("El correlativo es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Currency))
                errors.Add("La moneda es obligatoria.");

            if (!request.DocumentTotal.HasValue)
                errors.Add("El total del documento es obligatorio.");
            else if (request.DocumentTotal.Value <= 0)
                errors.Add("El total del documento debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(request.Comments))
                errors.Add("Los comentarios son obligatorios.");
        }

        /// <summary>
        /// Valida longitudes máximas permitidas para cada campo.
        /// </summary>
        private static void ValidateMaxLengths(CreateDocumentRequest request, List<string> errors)
        {
            ValidateMaxLength(request.CardCode, 20, "El código del proveedor", errors);
            ValidateMaxLength(request.Ruc, 10, "El RUC", errors);
            ValidateMaxLength(request.CardName, 150, "La razón social", errors);
            ValidateMaxLength(request.Currency, 3, "La moneda", errors);
            ValidateMaxLength(request.Comments, 254, "Los comentarios", errors);
            ValidateMaxLength(request.DocumentType, 2, "El tipo de documento", errors);
            ValidateMaxLength(request.Series, 4, "La serie", errors);
            ValidateMaxLength(request.Correlative, 8, "El correlativo", errors);
            ValidateMaxLength(request.RetentionTypeCode, 10, "El tipo de retención", errors);
            ValidateMaxLength(request.RetentionCode, 10, "El código de retención", errors);
        }

        /// <summary>
        /// Valida formatos básicos (numéricos, etc.).
        /// </summary>
        private static void ValidateFormats(CreateDocumentRequest request, List<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(request.Ruc) && !request.Ruc.All(char.IsDigit))
                errors.Add("El RUC solo debe contener números.");

            if (!string.IsNullOrWhiteSpace(request.Correlative) && !request.Correlative.All(char.IsDigit))
                errors.Add("El correlativo solo debe contener números.");
        }

        /// <summary>
        /// Valida consistencia de datos de retención.
        /// </summary>
        private static void ValidateRetention(CreateDocumentRequest request, List<string> errors)
        {
            var hasRetentionCode = !string.IsNullOrWhiteSpace(request.RetentionCode);
            var hasRetentionTypeCode = !string.IsNullOrWhiteSpace(request.RetentionTypeCode);

            if (hasRetentionCode && !hasRetentionTypeCode)
                errors.Add("El tipo de retención es obligatorio cuando se informa un código de retención.");

            if (hasRetentionTypeCode && !hasRetentionCode)
                errors.Add("El código de retención es obligatorio cuando se informa un tipo de retención.");
        }

        /// <summary>
        /// Valida la longitud máxima de un campo específico.
        /// </summary>
        private static void ValidateMaxLength(string? value, int maxLength, string fieldName, List<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
                errors.Add($"{fieldName} no puede superar los {maxLength} caracteres.");
        }
    }
}