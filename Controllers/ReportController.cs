using EcoMart.Data;
using EcoMart.Data.Enum;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Controllers
{
    public class ReportController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IReportRepository _reportRepository;
        private readonly ApplicationDbContext _context;

        public ReportController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IReportRepository reportRepository,ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _reportRepository = reportRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string term = "", string filterBy = "", int currentPage = 1)
        {
            // user info
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!string.IsNullOrEmpty(role)) role = char.ToUpper(role[0]) + role.Substring(1).ToLower();
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            // filter by
            ReportStatus? reportStatusFilter = null;
            if (!string.IsNullOrEmpty(filterBy)) {
                if (Enum.TryParse(filterBy, out ReportStatus parsedStatus)) {
                    reportStatusFilter = parsedStatus;
                }
            }

            int termInt;
            bool isTermValidInt = int.TryParse(term, out termInt);
            var rep = new ReportViewModel();

            var reports = (from report in _context.Reports
                          where (reportStatusFilter == null || report.ReportStatus == reportStatusFilter) && (string.IsNullOrEmpty(term) || report.Id == termInt)
                          orderby report.ReportStatus, report.ReportDate
                          select new Report {
                              Id = report.Id,
                              OrderId = report.OrderId,
                              ProductId = report.ProductId,
                              Product = report.Product,
                              ReportType = report.ReportType,
                              ReportText = report.ReportText,
                              ReportDate = report.ReportDate,
                              ReportStatus = report.ReportStatus
                          });

            if (!reports.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = reports.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            reports = reports.Skip((currentPage - 1) * pageSize).Take(pageSize);

            rep.Reports = reports;
            rep.Term = term;
            rep.FilterBy = filterBy;
            rep.CurrentPage = currentPage;
            rep.TotalPages = totalPage;
            rep.PageSize = pageSize;
            return View(rep);
        }

        [HttpGet]
        public async Task<IActionResult> GetReport(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null) return NotFound();

            var reportData = new {
                id = report.Id,
                orderId = report.OrderId,
                productId = report.ProductId,
                productName = report.Product.ProductName,
                reportType = report.ReportType,
                reportText = report.ReportText,
                reportDate = report.ReportDate,
                resolutionText = report.ResolutionText,
                resolutionDate = report.ResolutionDate
            };
            return Json(reportData);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReportViewModel reportViewModel)
        {
            var reportVM = reportViewModel.EditReportVM;
            if (reportVM == null) {
                TempData["ErrorMessage"] = "Failed to save the product!";
                return RedirectToAction("Index");
            }

            var editReport = await _reportRepository.GetByIdAsyncNoTracking(id);
            if (editReport != null) {
                var report = new Report {
                    Id = id,
                    OrderId = reportVM.OrderId,
                    ProductId = reportVM.ProductId,
                    ReportType = reportVM.ReportType,
                    ReportText = reportVM.ReportText,
                    ReportDate = reportVM.ReportDate,
                    ReportStatus = ReportStatus.Resolved,
                    ResolutionText = reportVM.ResolutionText,
                    ResolutionDate = DateTime.Now
                };
                _reportRepository.Update(report);
                TempData["SuccessMessage"] = "Report product resolved successfully!";
                return RedirectToAction("Index");
            } else {
                TempData["ErrorMessage"] = "Report not found!";
                return RedirectToAction("Index");
            }
        }
    }
}
