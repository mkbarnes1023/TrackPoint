namespace TrackPoint.Models
{
    public class UserPreferencesModel
    {
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
