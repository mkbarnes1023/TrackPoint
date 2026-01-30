using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TrackPoint.Models
{
    public class UserPreferences
    {
        [Key]
        public int preferenceId { get; set; }
        public int userId { get; set; }
        public string dashboardType { get; set; } = string.Empty;
        public string visibleColumns { get; set; } = string.Empty;
        public string filters { get; set; } = string.Empty;
        public string sortOrder { get; set; } = string.Empty;
        public string layout { get; set; } = string.Empty;
        public DateTime lastUpdated { get; set; }

    }
}
