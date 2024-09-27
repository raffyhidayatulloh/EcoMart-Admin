using EcoMart.Data;
using EcoMart.Data.Enum;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Controllers
{
    public class IncomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;

        public IncomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPaymentRepository paymentRepository, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string filterBy = "")
        {
            // user info
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!string.IsNullOrEmpty(role)) role = char.ToUpper(role[0]) + role.Substring(1).ToLower();
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            var incData = new IncomeViewModel();

            var currentYear = DateTime.Today.Year;
            if (!string.IsNullOrEmpty(filterBy) && int.TryParse(filterBy, out int selectedYear)) {
                currentYear = selectedYear;
            }
            filterBy = currentYear.ToString();

            var paymentsPerMonth = _context.Payments.Where(pay => pay.PaymentDate.HasValue && pay.PaymentDate.Value.Year == currentYear)
                .GroupBy(pay => pay.PaymentDate.Value.Month)
                .Select(g => new {
                    Month = g.Key,
                    TotalAmount = g.Sum(p => p.PaymentAmount)
                }).OrderBy(g => g.Month).ToList();

            int[] monthlyPayments = new int[12];
            int totalIncome = 0;
            foreach (var item in paymentsPerMonth) {
                monthlyPayments[item.Month - 1] = item.TotalAmount;
                totalIncome += item.TotalAmount;
            }
            ViewBag.MonthlyPayments = monthlyPayments;

            var years = Enumerable.Range(2020, DateTime.Now.Year - 2019).ToList();
            ViewBag.Years = years;

            incData.TotalIncome = totalIncome;
            incData.FilterBy = filterBy;
            return View(incData);
        }
    }
}
