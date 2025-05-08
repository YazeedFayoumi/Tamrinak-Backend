using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Sport
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SportId { get; set; }

		[Required, MaxLength(50)]
		public string Name { get; set; }

		public string Description { get; set; }

		public ICollection<SportFacility> SportFacilities { get; set; } = new List<SportFacility>();
		public List<Field> Fields { get; set; } = new List<Field>();
		public ICollection<Image> Images { get; set; } = new List<Image>(); //TODO only icons
	}

}
