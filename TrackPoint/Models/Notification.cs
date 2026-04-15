using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TrackPoint.Models
{
    public class Notification
    {
        [Key]
        public int notificationId { get; set; }

        public string? userId { get; set; }

        [ForeignKey(nameof(userId))]
        public IdentityUser? User { get; set; }

        public string type { get; set; } = string.Empty;
        public int assetId { get; set; }
        public int pendingApprovalId { get; set; }
        public string title { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public DateTime readAt { get; set; }
        public DateTime emailedAt { get; set; }

    }
}
