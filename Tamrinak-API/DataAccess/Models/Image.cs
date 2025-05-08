namespace Tamrinak_API.DataAccess.Models
{
	public class Image
	{
		public int Id { get; set; }

		public string Base64Data { get; set; }

		public int? FacilityId { get; set; }
		public Facility? Facility { get; set; }

		public int? FieldId { get; set; }
		public Field? Field { get; set; }

		public int? SportId { get; set; }//TODO
		public Sport? Sport { get; set; }
	}
}
