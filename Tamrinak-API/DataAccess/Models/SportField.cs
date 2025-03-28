using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tamrinak_API.DataAccess.Models
{
    public class SportField
    {
        [Required]
        public int FieldId { get; set; }

        
        public Field Field { get; set; }

        [Required]
        public int SportId { get; set; }

        
        public Sport Sport { get; set; }
    }
}
