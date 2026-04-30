namespace DocGenerator.Domain.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime? PostingDate { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? DueDate { get; set; }

        public string? CardCode { get; set; }
        public string? Ruc { get; set; }
        public string? CardName { get; set; }

        public string? Currency { get; set; }
        public string? Comments { get; set; }

        public string? DocumentType { get; set; }
        public string? Series { get; set; }
        public string? Correlative { get; set; }

        public bool? IsValidSunat { get; set; }

        public string? RetentionTypeCode { get; set; }
        public string? RetentionCode { get; set; }

        public decimal? DocumentTotal { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User? User { get; set; }
    }
}
