using EcoMart.Data;
using EcoMart.Data.Enum;
using EcoMart.Models;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcoMart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // info user
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!string.IsNullOrEmpty(role)) {
                role = char.ToUpper(role[0]) + role.Substring(1).ToLower();
            }
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            var currentYear = DateTime.Today.Year;
            var currentMonth = DateTime.Now.Month;

            // data chart bar
            var paymentsPerMonth = _context.Payments.Where(pay => pay.PaymentDate.HasValue && pay.PaymentDate.Value.Year == currentYear).GroupBy(pay => pay.PaymentDate.Value.Month)
                .Select(g => new {
                    Month = g.Key,
                    TotalAmount = g.Sum(p => p.PaymentAmount)
                }).OrderBy(g => g.Month).ToList();
            int[] monthlyPayments = new int[12];
            foreach (var item in paymentsPerMonth) {
                monthlyPayments[item.Month - 1] = item.TotalAmount;
            }
            ViewBag.MonthlyPayments = monthlyPayments;

            var dashData = new DashboardViewModel();

            // data 4 card
            var queryPaymentSuccess = _context.Payments.Where(pay => pay.PaymentDate >= DateTime.Today && pay.PaymentDate < DateTime.Today.AddDays(1) && pay.PaymentStatus == PaymentStatus.Completed);
            var queryNotYetPaid = _context.Payments.Where(pay => pay.PaymentDate >= DateTime.Today && pay.PaymentDate < DateTime.Today.AddDays(1) && pay.PaymentStatus == PaymentStatus.Failed);
            var queryOrderPending = _context.Orders.Where(ord => ord.OrderDate >= DateTime.Today && ord.OrderDate < DateTime.Today.AddDays(1) && ord.OrderStatus == OrderStatus.Pending);
            var queryOrderDelivered = _context.Shippings.Where(shi => shi.DeliveredDate >= DateTime.Today && shi.DeliveredDate < DateTime.Today.AddDays(1));
            dashData.PaidSuccessfully = queryPaymentSuccess.Count();
            dashData.NotYetPaid = queryNotYetPaid.Count();
            dashData.OrderPending = queryOrderPending.Count();
            dashData.OrderDelivered = queryOrderDelivered.Count();

            // data order statistics
            var orderStatistics = (from order in _context.Orders
                                   where order.OrderStatus == OrderStatus.Delivered && order.PaymentStatus == PaymentStatus.Completed && 
                                         order.OrderDate.Month == currentMonth && order.OrderDate.Year == currentYear
                                   select new Order {
                                       Id = order.Id
                                   });
            if (!orderStatistics.Any()) {
                dashData.TotalOrders = 0;
            } else {
                dashData.TotalOrders = orderStatistics.Count();
            }
            var categoryStatistics = (from od in _context.OrderDetails
                                      join p in _context.Products on od.ProductId equals p.Id
                                      join c in _context.Categories on p.CategoryId equals c.Id
                                      where orderStatistics.Select(o => o.Id).Contains(od.OrderId)
                                      group od by c.CategoryName into categoryGroup
                                      select new {
                                          CategoryName = categoryGroup.Key,
                                          TopCategory = categoryGroup.Sum(od => od.Quantity)
                                      }).OrderByDescending(cg => cg.TopCategory).Take(3).ToList();
            var topCategoryList = new List<string>();
            var categoryCountList = new List<int>();
            foreach (var item in categoryStatistics) {
                topCategoryList.Add(item.CategoryName);
                categoryCountList.Add(item.TopCategory);
            }
            ViewBag.TopCategoryMonthly = topCategoryList.ToArray();
            ViewBag.CategoryCountMonthly = categoryCountList.ToArray();

            // data order pending
            var orders = (from order in _context.Orders
                          where order.OrderStatus == OrderStatus.Pending && order.PaymentStatus == PaymentStatus.Completed
                          orderby order.OrderDate descending
                          select new Order {
                              Id = order.Id,
                              OrderDate = order.OrderDate,
                              OrderStatus = order.OrderStatus,
                              TotalAmount = order.TotalAmount,
                              PaymentStatus = order.PaymentStatus,
                              ShippingAddress = order.ShippingAddress,
                              CreatedAt = order.CreatedAt,
                              UpdatedAt = order.UpdatedAt,
                          }).Take(5);
            if (!orders.Any()) orders = null;
            dashData.Orders = orders;

            // best product
            var bestProducts = (from order in _context.Orders
                                join orderDetail in _context.OrderDetails
                                on order.Id equals orderDetail.OrderId
                                join product in _context.Products
                                on orderDetail.ProductId equals product.Id
                                join category in _context.Categories
                                on product.CategoryId equals category.Id
                                where order.OrderStatus == OrderStatus.Delivered && order.PaymentStatus == PaymentStatus.Completed && order.OrderDate.Month == currentMonth && order.OrderDate.Year == currentYear
                                group orderDetail by new { product.ProductName, product.ImageUrl, category.CategoryName } into productGroup
                                select new BestSellerProducts {
                                    ProductName = productGroup.Key.ProductName,
                                    ImageUrl = productGroup.Key.ImageUrl,
                                    CategoryName = productGroup.Key.CategoryName,
                                    TotalQuantity = productGroup.Sum(od => od.Quantity)
                                }).OrderByDescending(p => p.TotalQuantity).Take(4);
            dashData.BestProducts = bestProducts;

            // top product
            var topProducts = (from pro in _context.Products
                               join rev in _context.Reviews on pro.Id equals rev.ProductId into reviewsGroup
                               from review in reviewsGroup.DefaultIfEmpty()
                               where review == null || review.Rating > 0
                               group review by new {
                                   pro.ProductName,
                                   pro.ImageUrl
                               } into grouped
                               select new TopRatedProducts {
                                   ProductName = grouped.Key.ProductName,
                                   ImageUrl = grouped.Key.ImageUrl,
                                   AverageRating = grouped.Average(r => r != null ? r.Rating : 0),
                                   TotalReviews = grouped.Count(r => r != null && r.Rating != null)
                               }).Where(r => r.AverageRating > 0).OrderByDescending(r => r.AverageRating).ThenByDescending(r => r.TotalReviews).Take(4);
            dashData.TopProducts = topProducts;

            return View(dashData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
