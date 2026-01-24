using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TrackPoint.Models
{
    public class AssetLoan
    {
        [Key]
        public int loanId { get; set; }

        //foreign key to Asset
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        //foreign key to IdentityUser for borrowerId
        public string BorrowerId { get; set; }
        public IdentityUser Borrower { get; set; }

        public DateTime CheckedoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }

        // foreign key to IdentityUser for extendedByAdminId
        public string? ExtendedByAdminId { get; set; }
        public IdentityUser? ExtendedBy { get; set; }

        //foreign key to IdentityUser for approvedByUserId
        public string ApprovedByUserId { get; set; }
        public IdentityUser ApprovedBy { get; set; }


    }
}
