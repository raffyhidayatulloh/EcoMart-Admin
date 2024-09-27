using CloudinaryDotNet.Actions;
using EcoMart.Data;
using EcoMart.Data.Enum;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcoMart.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public PaymentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPaymentRepository paymentRepository, IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _context = context;
        }

        [Authorize]
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
            PaymentStatus? paymentStatusFilter = null;
            if (!string.IsNullOrEmpty(filterBy)) {
                if (Enum.TryParse(filterBy, out PaymentStatus parsedStatus)) {
                    paymentStatusFilter = parsedStatus;
                }
            }

            int termInt;
            bool isTermValidInt = int.TryParse(term, out termInt);
            var payData = new PaymentViewModel();

            var payments = (from pay in _context.Payments
                            where (paymentStatusFilter == null || pay.PaymentStatus == paymentStatusFilter) && 
                                  (string.IsNullOrEmpty(term) || pay.OrderId == termInt)
                            orderby pay.PaymentStatus descending, pay.PaymentDate descending
                            select new Payment {
                                Id = pay.Id,
                                OrderId = pay.OrderId,
                                PaymentDate = pay.PaymentDate,
                                PaymentAmount = pay.PaymentAmount,
                                PaymentMethod = pay.PaymentMethod,
                                PaymentStatus = pay.PaymentStatus,
                                CreatedAt = pay.CreatedAt,
                                UpdatedAt = pay.UpdatedAt
                            });

            if (!payments.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = payments.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            payments = payments.Skip((currentPage - 1) * pageSize).Take(pageSize);

            payData.Payments = payments;
            payData.Term = term;
            payData.FilterBy = filterBy;
            payData.CurrentPage = currentPage;
            payData.TotalPages = totalPage;
            payData.PageSize = pageSize;
            return View(payData);
        }
    }
}