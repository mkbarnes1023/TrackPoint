using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackPoint.Models
{
    public class Office
    {
        public int OfficeId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Foreign key to Location
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; }
    }
}
