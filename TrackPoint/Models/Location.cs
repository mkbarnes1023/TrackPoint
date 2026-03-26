using System.ComponentModel.DataAnnotations;
namespace TrackPoint.Models;

public class Location
{
    [Key]
    public int LocationId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Abbreviation { get; set; }

    public Office Office { get; set; }
    public int DefaultOfficeId { get; set; } = 0; // By default, the office ID is unassigned

    // Navigation property
    public ICollection<Asset>? Assets { get; set; }
}
