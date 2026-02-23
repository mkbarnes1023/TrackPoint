using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static TrackPoint.Models.Enums;

namespace TrackPoint.Models
{
    public class TransferLog
    {
        [Key]
        public int TransferLogId { get; set; }

        // Foreign key to Asset
        [Required]
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        // Foreign key to IdentityUser for issuedTo
        [Required]
        public string BorrowerId { get; set; }
        public IdentityUser Borrower { get; set; }

        
        // Foreign key for Asset Status
        public string NewStatus { get; set; }
        public Asset AssetStatus { get; set; }
        public string OldStatus { get; set; } // Status before the change, if applicable
    
        [Required]
        public eventType eventType { get; set; } // TODO: Confirm that this works with Enums.cs
        public DateTime TransferDate { get; set; }
}
}
