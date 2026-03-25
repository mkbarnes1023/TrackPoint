using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
    public class Office
    {
        [Key]
        public int OfficeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public Location Location { get; set; }
        public int LocationId { get; set; }
    }
}
