namespace DocGenerator.Infrastructure.Repositories.Commons
{
    public interface ICommonRepository
    {
        /// <summary>
        /// Valida si la moneda existe en SAP.
        /// </summary>
        Task<bool> ExistsCurrencyInSapAsync(string currency);

        /// <summary>
        /// Valida si el código de retención existe en SAP.
        /// </summary>
        Task<bool> ExistsRetentionCodeInSapAsync(string retentionCode);
    }
}
