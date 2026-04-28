namespace DocGenerator.Application.Settings
{
    public class DocGeneratorWebSettings
    {
        public string DevLink { get; set; } = string.Empty;
        public string ProdLink { get; set; } = string.Empty;
        public bool IsProd { get; set; }
    }
}
