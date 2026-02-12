using System.Collections.Generic;

namespace TrackPoint.Models.DTOs
{
    public class AdminDashboardViewModel
    {
        public List<CountByLabelDto> StatusCounts { get; set; } = new();
        public int UnassignedCount { get; set; }
    }
}