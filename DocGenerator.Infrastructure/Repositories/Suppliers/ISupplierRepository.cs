namespace DocGenerator.Infrastructure.Repositories.Suppliers
{
    public interface ISupplierRepository
    {
        /// <summary>
        /// Valida si el proveedor existe en SAP
        /// </summary>
        Task<bool> ExistsSupplierInSapAsync(string cardCode);

        /// <summary>
        /// Valida que el CardCode corresponda al RUC en SAP
        /// </summary>
        Task<bool> IsSupplierRucMatchAsync(string cardCode, string ruc);

        /// <summary>
        /// Valida si el proveedor tiene habilitado el código de retención en SAP
        /// </summary>
        Task<bool> SupplierHasRetentionCodeAsync(string supplierCode, string retentionCode);
    }
}
