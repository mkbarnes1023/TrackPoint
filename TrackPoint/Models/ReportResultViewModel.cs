namespace TrackPoint.Models
{
    public class ReportResultViewModel
    {
        public string ReportType { get; set; }          // Category, Location, Status
        public bool IncludeKpiSummary { get; set; }         // KPI summary page
        public bool IncludeTableSummary { get; set; }                 // table summary
        public string ExportType { get; set; }
        public List<ReportRowViewModel> Rows { get; set; } = new List<ReportRowViewModel>();

        // KPI data for charts (pie / bar)
        public List<ChartPointViewModel> PieSeries { get; set; } = new List<ChartPointViewModel>();
        public List<ChartPointViewModel> BarSeries { get; set; } = new List<ChartPointViewModel>();
    }
}

