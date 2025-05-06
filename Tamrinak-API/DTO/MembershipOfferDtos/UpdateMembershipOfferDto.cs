using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.MembershipOfferDtos
{
    public class UpdateMembershipOfferDto
    {
        [Required]
        public int DurationInMonths { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
