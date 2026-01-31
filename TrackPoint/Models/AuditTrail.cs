using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackPoint.Models
{
    public class AuditTrail

    {
        [Key]
        public int AuditID { get; set; }

        //foreign key to Asset
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        public string NewStatus { get; set; }

        //foreign key to IdentityUser for changedByUserId
        public string ChangedByUserId { get; set; }
        public IdentityUser ChangedBy { get; set; }

        public DateTime? ChangeDate { get; set; }
        public string? Comment { get; set; }

        //foreign key to Approvals
        public int RelatedApprovalId { get; set; }
        public ApprovalReason ApprovalReason { get; set; }
        public string FieldChanged { get; set; }
        public string? NewValue { get; set; }

    }
}