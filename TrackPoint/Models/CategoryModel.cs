using System.ComponentModel.DataAnnotations;
using TrackPoint.Models;

public class CategoryModel
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

    // Navigation property - optional but helpful
    public ICollection<Asset>? Assets { get; set; }
}
