
namespace Tamrinak_API.DTO
{
    public class LoginResponse
    {
        public required string JwtToken {  get; set; }   
        public DateTime Expiration {  get; set; }

        public static implicit operator Task<object>(LoginResponse v)
        {
            throw new NotImplementedException();
        }
    }
}
