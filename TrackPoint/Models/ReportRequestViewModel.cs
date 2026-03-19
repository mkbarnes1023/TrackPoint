namespace TrackPoint.Models;

public class ReportRequestViewModel
{
    public string ReportType { get; set; }          // Category, Location, Status
    public bool Visualization { get; set; }         // KPI summary page
    public bool Table { get; set; }                 // table summary
    public string ExportType { get; set; }          // Excel, PDF
}