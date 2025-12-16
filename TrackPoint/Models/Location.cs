using System.ComponentModel.DataAnnotations;
using TrackPoint.Models;

public class Location
{
    [Key]
    public int LocationId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Abbreviation { get; set; }

    // Navigation property (optional but recommended)
    public ICollection<Asset>? Assets { get; set; }
}
