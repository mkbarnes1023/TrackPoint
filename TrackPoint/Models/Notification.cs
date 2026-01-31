using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
    public class Notification
    {
        [Key]
        public int notificationId { get; set; }

        public int userId { get; set; }
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
