using DocGenerator.Application.DTOs.Documents;

namespace DocGenerator.Application.Helpers.Documents
{
    public static class DocumentDetailValidator
    {
        private const decimal AmountTolerance = 0.01m;

        /// <summary>
        /// Ejecuta las validaciones para una lista de detalles en creación conjunta con la cabecera.
        /// No valida DocId porque se asigna después de crear la cabecera.
        /// </summary>
        public static List<string> ValidateCreate(List<CreateDocDetailRequest>? details)
        {
            var errors = new List<string>();

            if (details == null || !details.Any())
            {
                errors.Add("Debe ingresar al menos un detalle para el documento.");
                return errors;
            }

            for (int i = 0; i < details.Count; i++)
            {
                var lineNumber = i + 1;
                var detail = details[i];

                if (detail == null)
                {
                    errors.Add($"La línea {lineNumber} del detalle es obligatoria.");
                    continue;
                }

                Normalize(detail);

                ValidateRequiredFields(detail, errors, lineNumber);
                ValidateMaxLengths(detail, errors, lineNumber);
                ValidateAmounts(detail, errors, lineNumber);
                ValidateTax(detail, errors, lineNumber);
            }

            return errors;
        }

        /// <summary>
        /// Normaliza textos del detalle aplicando Trim y mayúsculas donde corresponde.
        /// </summary>
        private static void Normalize(CreateDocDetailRequest detail)
        {
            detail.ItemCode = detail.ItemCode?.Trim().ToUpper();
            detail.TaxCode = detail.TaxCode?.Trim().ToUpper();
            detail.ProjectCode = detail.ProjectCode?.Trim().ToUpper();

            detail.Dim1 = detail.Dim1?.Trim().ToUpper();
            detail.Dim2 = detail.Dim2?.Trim().ToUpper();
            detail.Dim3 = detail.Dim3?.Trim().ToUpper();
            detail.Dim4 = detail.Dim4?.Trim().ToUpper();
            detail.Dim5 = detail.Dim5?.Trim().ToUpper();

            detail.WarehouseCode = detail.WarehouseCode?.Trim().ToUpper();
        }

        /// <summary>
        /// Valida campos obligatorios del detalle.
        /// </summary>
        private static void ValidateRequiredFields(CreateDocDetailRequest detail, List<string> errors, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(detail.ItemCode))
                errors.Add($"Línea {lineNumber}: El código de artículo es obligatorio.");

            if (!detail.Quantity.HasValue)
                errors.Add($"Línea {lineNumber}: La cantidad es obligatoria.");

            if (!detail.Price.HasValue)
                errors.Add($"Línea {lineNumber}: El precio es obligatorio.");

            if (!detail.Subtotal.HasValue)
                errors.Add($"Línea {lineNumber}: El subtotal es obligatorio.");

            if (!detail.Total.HasValue)
                errors.Add($"Línea {lineNumber}: El total es obligatorio.");
        }

        /// <summary>
        /// Valida longitudes máximas permitidas para los campos de texto del detalle.
        /// </summary>
        private static void ValidateMaxLengths(CreateDocDetailRequest detail, List<string> errors, int lineNumber)
        {
            ValidateMaxLength(detail.ItemCode, 50, "El código de artículo", errors, lineNumber);
            ValidateMaxLength(detail.TaxCode, 10, "El código de impuesto", errors, lineNumber);
            ValidateMaxLength(detail.ProjectCode, 20, "El proyecto", errors, lineNumber);

            ValidateMaxLength(detail.Dim1, 10, "La dimensión 1", errors, lineNumber);
            ValidateMaxLength(detail.Dim2, 10, "La dimensión 2", errors, lineNumber);
            ValidateMaxLength(detail.Dim3, 10, "La dimensión 3", errors, lineNumber);
            ValidateMaxLength(detail.Dim4, 10, "La dimensión 4", errors, lineNumber);
            ValidateMaxLength(detail.Dim5, 10, "La dimensión 5", errors, lineNumber);

            ValidateMaxLength(detail.WarehouseCode, 10, "El almacén", errors, lineNumber);
        }

        /// <summary>
        /// Valida valores numéricos y consistencia entre cantidad, precio, subtotal y total.
        /// </summary>
        private static void ValidateAmounts(CreateDocDetailRequest detail, List<string> errors, int lineNumber)
        {
            if (detail.Quantity.HasValue && detail.Quantity.Value <= 0)
                errors.Add($"Línea {lineNumber}: La cantidad debe ser mayor a 0.");

            if (detail.Price.HasValue && detail.Price.Value < 0.1m)
                errors.Add($"Línea {lineNumber}: El precio debe ser mayor o igual a 0.1.");

            if (detail.Subtotal.HasValue && detail.Subtotal.Value <= 0)
                errors.Add($"Línea {lineNumber}: El subtotal debe ser mayor a 0.");

            if (detail.Total.HasValue && detail.Total.Value <= 0)
                errors.Add($"Línea {lineNumber}: El total debe ser mayor a 0.");

            if (detail.Quantity.HasValue && detail.Price.HasValue && detail.Subtotal.HasValue)
            {
                var expectedSubtotal = detail.Quantity.Value * detail.Price.Value;
                var difference = Math.Abs(detail.Subtotal.Value - expectedSubtotal);

                if (difference > AmountTolerance)
                    errors.Add($"Línea {lineNumber}: El subtotal no coincide con cantidad por precio.");
            }

            if (detail.Subtotal.HasValue && detail.Total.HasValue && detail.Total.Value < detail.Subtotal.Value)
                errors.Add($"Línea {lineNumber}: El total no puede ser menor que el subtotal.");

            if (
                detail.Quantity.GetValueOrDefault() == 0 &&
                detail.Price.GetValueOrDefault() == 0 &&
                detail.Subtotal.GetValueOrDefault() == 0 &&
                detail.Total.GetValueOrDefault() == 0
            )
            {
                errors.Add($"Línea {lineNumber}: No se permite registrar una línea vacía o sin importes.");
            }
        }

        /// <summary>
        /// Valida la coherencia del impuesto respecto al subtotal y total.
        /// </summary>
        private static void ValidateTax(CreateDocDetailRequest detail, List<string> errors, int lineNumber)
        {
            var hasTaxCode = !string.IsNullOrWhiteSpace(detail.TaxCode);

            if (!detail.Subtotal.HasValue || !detail.Total.HasValue)
                return;

            if (hasTaxCode && detail.Total.Value <= detail.Subtotal.Value)
                errors.Add($"Línea {lineNumber}: Si tiene impuesto, el total debe ser mayor que el subtotal.");

            if (!hasTaxCode && Math.Abs(detail.Total.Value - detail.Subtotal.Value) > AmountTolerance)
                errors.Add($"Línea {lineNumber}: Si no tiene impuesto, el total debe ser igual al subtotal.");
        }

        /// <summary>
        /// Valida la longitud máxima de un campo específico del detalle.
        /// </summary>
        private static void ValidateMaxLength(string? value, int maxLength, string fieldName, List<string> errors, int lineNumber)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
                errors.Add($"Línea {lineNumber}: {fieldName} no puede superar los {maxLength} caracteres.");
        }
    }
}
