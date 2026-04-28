namespace DocGenerator.Domain.Entities
{
    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string TokenType { get; set; }
        public bool IsUsed { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
