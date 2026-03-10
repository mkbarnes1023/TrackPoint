using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrackPoint.Data;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ReportBuilder()
        {
            var model = new ReportRequestViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult ReportBuilder(ReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var report = new ReportResultViewModel
            {
                ReportType = request.ReportType,
                IncludeKpiSummary = request.Visualization,
                IncludeTableSummary = request.Table,
                ExportType = request.ExportType
            };

            if (request.ReportType == "Category")
            {
                var assets = _context.Asset
                    .Include(a => a.Category)
                    .ToList();

                var groups = assets
                    .GroupBy(a => a.Category != null ? a.Category.Name : "Uncategorized")
                    .Select(g =>
                    {
                        var quantity = g.Count();

                        var avgTotal = quantity > 0
                            ? decimal.Round(g.Average(a => a.PurchasePrice), 2)
                            : 0m;

                        var maintenanceCount = g.Count(a =>
                            !string.IsNullOrEmpty(a.AssetStatus) &&
                            a.AssetStatus.ToLower().Contains("maint"));

                        var maintenancePct = quantity > 0
                            ? decimal.Round((decimal)maintenanceCount / quantity * 100m, 2)
                            : 0m;

                        return new ReportRowViewModel
                        {
                            Name = g.Key,
                            Quantity = quantity,
                            AverageTotal = avgTotal,
                            AverageMaintenance = maintenancePct
                        };
                    })
                    .OrderByDescending(r => r.Quantity)
                    .ToList();

                report.Rows.AddRange(groups);
            }

            if (request.ExportType == "PDF")
            {
                return ExportPdf(report);
            }

            return View("ReportResults", report);
        }

        private IActionResult ExportPdf(ReportResultViewModel report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"{report.ReportType} Report")
                        .FontSize(20)
                        .Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Border(1).Padding(5).Text("Category").Bold();
                            header.Cell().Border(1).Padding(5).Text("Quantity").Bold();
                            header.Cell().Border(1).Padding(5).Text("Average").Bold();
                            header.Cell().Border(1).Padding(5).Text("% Maintenance").Bold();
                        });

                        foreach (var row in report.Rows)
                        {
                            table.Cell().Border(1).Padding(5).Text(row.Name);
                            table.Cell().Border(1).Padding(5).Text(row.Quantity.ToString());
                            table.Cell().Border(1).Padding(5).Text(row.AverageTotal.ToString("C"));
                            table.Cell().Border(1).Padding(5).Text($"{row.AverageMaintenance}%");
                        }
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"{report.ReportType}_Report.pdf");
        }
    }
}