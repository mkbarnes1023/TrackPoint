using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TrackPoint.Models;

public class Asset
{
    [Key]
    public int AssetId { get; set; }

    [Required]
    [StringLength(100)]
    public string AssetTag { get; set; }

    [Required]
    [StringLength(100)]
    public string Make { get; set; }

    [Required]
    [StringLength(100)]
    public string Model { get; set; }

    // ---------------------
    // Foreign Keys
    // ---------------------

    // Issued to: Identity user FK
    public string? IssuedToUserId { get; set; }

    [ForeignKey(nameof(IssuedToUserId))]
    public IdentityUser? IssuedToUser { get; set; }

    // Category FK
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // Location FK
    public int LocationId { get; set; }
    public Location Location { get; set; }

    // ---------------------
    // Status Fields
    // ---------------------

    [Required]
    public string AssetStatus { get; set; }

    public string? Notes { get; set; }

    public DateTime StatusDate { get; set; }

    public DateTime? WarrantyExpirationDate { get; set; }

    // ---------------------
    // Asset Details
    // ---------------------

    public string SerialNumber { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    public string Vendor { get; set; }

    public string Condition { get; set; }
}
