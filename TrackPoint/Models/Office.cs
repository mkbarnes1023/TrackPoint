using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
    public class Office
    {
        public int OfficeId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
