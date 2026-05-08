namespace BudgetTracker.DTOs.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; }
    }
}
