namespace DocGenerator.Application.DTOs.Documents
{
    public class UpdateDocDetailRequest
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }

        public string? ItemCode { get; set; }

        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Subtotal { get; set; }

        public string? TaxCode { get; set; }
        public decimal? Total { get; set; }

        public string? ProjectCode { get; set; }

        public string? Dim1 { get; set; }
        public string? Dim2 { get; set; }
        public string? Dim3 { get; set; }
        public string? Dim4 { get; set; }
        public string? Dim5 { get; set; }

        public string? WarehouseCode { get; set; }
    }
}
