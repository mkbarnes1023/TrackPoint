using System.ComponentModel.DataAnnotations;
namespace TrackPoint.Models;


public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Abbreviation { get; set; }

    [Required]
    public bool RequiresApproval { get; set; }

    public int? DefaultLoanPeriodDays { get; set; }

    public string? Description { get; set; }

    [Required]
    public bool ContainsConsumables { get; set; }

    // Category Icon using the Bootstrap Icons library
    [StringLength(50)]
    public string? Icon { get; set; }

    // Navigation property - optional but helpful
    public ICollection<Asset>? Assets { get; set; }
}
