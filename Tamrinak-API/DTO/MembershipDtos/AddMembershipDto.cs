using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DTO.MembershipDtos
{
	public class AddMembershipDto
	{
		[Required]
		public int FacilityId { get; set; }
        public int? OfferId { get; set; } = null;
    }
}
