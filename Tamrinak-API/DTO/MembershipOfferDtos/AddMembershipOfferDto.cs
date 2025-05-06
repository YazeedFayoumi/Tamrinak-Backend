using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.MembershipOfferDtos
{
    public class AddMembershipOfferDto
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public int DurationInMonths { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
