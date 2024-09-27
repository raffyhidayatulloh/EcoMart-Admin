using EcoMart.Data.Enum;

namespace EcoMart.ViewModels
{
    public class EditReportViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ReportType { get; set; }
        public string ReportText { get; set; }
        public DateTime ReportDate { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public string ResolutionText { get; set; }
        public DateTime ResolutionDate { get; set; }
    }
}
