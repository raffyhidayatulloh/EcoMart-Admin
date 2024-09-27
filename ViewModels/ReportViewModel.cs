using EcoMart.Models;

namespace EcoMart.ViewModels
{
    public class ReportViewModel
    {
        public IQueryable<Report> Reports { get; set; }
        public string Term { get; set; }
        public string FilterBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public EditReportViewModel EditReportVM { get; set; }
    }
}
