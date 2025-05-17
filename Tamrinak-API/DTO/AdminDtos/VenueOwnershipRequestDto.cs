namespace Tamrinak_API.DTO.AdminDtos
{
    public class VenueOwnershipRequestDto
    {
        public int VenueId { get; set; }
        public string VenueType { get; set; } // "Field" or "Facility"  مرفق/ نادي
    }
}
