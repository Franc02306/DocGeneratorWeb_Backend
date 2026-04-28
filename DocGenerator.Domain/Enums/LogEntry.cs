namespace DocGenerator.Domain.Enums
{
    /// <summary>
    /// Representa la información de un log HTTP
    /// </summary>
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public string Module { get; set; } = "General";
        public string Action { get; set; } = "Unknown";
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public int StatusCode { get; set; }
        public string? User { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? Error { get; set; }
    }
}
