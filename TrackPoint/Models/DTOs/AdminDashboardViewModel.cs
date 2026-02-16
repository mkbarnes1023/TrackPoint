using System.Collections.Generic;

namespace TrackPoint.Models.DTOs
{
    public class AdminDashboardViewModel
    {
        public List<CountByLabelDto> StatusCounts { get; set; } = new();
        public int UnassignedCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public int ExpiredCount { get; set; }
        public int ZeroToThirty { get; set; }
        public int ThirtyOneToNinety { get; set; }
        public int NinetyPlus { get; set; }

        // Count of assets with warranty/expiration within the next 6 months
        public int WarrantyExpiringWithin6MonthsCount { get; set; }

        // Optional: breakdown by label (e.g., location or category) for those expiring assets
        public List<CountByLabelDto> WarrantyExpiringByLabel { get; set; } = new();
    }
}