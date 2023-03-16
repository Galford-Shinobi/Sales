using Sales.Shared.Responses;

namespace Sales.Shared.DTOs
{
    public class TokenDTO
    {
        public string Token { get; set; } = null!;

        public UserResponse User { get; set; }

        public DateTime Expiration { get; set; }

        public DateTime ExpirationLocal => Expiration.ToLocalTime();
    }
}
