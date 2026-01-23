using Microsoft.AspNetCore.Identity;

namespace TrackPoint.Models
{
    public class Approvals
    {
        public int approvalId { get; set; }
        public int reasonId { get; set; }
        public IdentityUser? requestorId { get; set; }
        public int assetId { get; set; }
        public DateTime? requestDate { get; set; }
        public string approvalStatus { get; set; }
        public IdentityUser? approvedByUserId { get; set; }
        public DateTime? resolvedDate { get; set; }
        public string comments { get; set; }
        public string approvalRelatedStatus { get; set; }


    }
}
