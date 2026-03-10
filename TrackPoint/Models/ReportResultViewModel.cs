namespace TrackPoint.Models
{
    public class ReportResultViewModel
    {
        public string ReportType { get; set; }          // Category, Location, Status
        public bool IncludeKpiSummary { get; set; }         // KPI summary page
        public bool IncludeTableSummary { get; set; }                 // table summary
        public string ExportType { get; set; }
        public List<ReportRowViewModel> Rows { get; set; } = new List<ReportRowViewModel>();
    }
}

