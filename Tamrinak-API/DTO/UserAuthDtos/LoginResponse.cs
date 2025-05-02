namespace Tamrinak_API.DTO.UserAuthDtos
{
    public class LoginResponse
    {
        public required string JwtToken { get; set; }
        public DateTime Expiration { get; set; }


    }
}
