using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TrackPoint.Models
{
    public class Approvals
    {
        [Key]
        public int ApprovalId { get; set; }
        // Foreign key to ApprovalReason
        public int ReasonId { get; set; }
        public ApprovalReason ApprovalReason { get; set; }

        // Foreign key to IdentityUser for requestor
        public string RequestorId { get; set; }
        public IdentityUser Requestor { get; set; }

        // Foreign key to Asset
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public DateTime RequestDate { get; set; }
        public string ApprovalStatus { get; set; }

        //Foreign key to IdentityUser for approver
        public string? ApproverId { get; set; }
        public IdentityUser? Approver { get; set; }

        public DateTime? ResolvedDate { get; set; }
        public string? Comments { get; set; }
        public string ApprovalRelatedStatus { get; set; }


    }
}
