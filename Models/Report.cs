using EcoMart.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMart.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public string ReportType { get; set; }

        public string ReportText { get; set; }

        public DateTime ReportDate { get; set; }

        public ReportStatus ReportStatus { get; set; }

        public string? ResolutionText { get; set; }

        public DateTime? ResolutionDate { get; set; }
    }
}
