using EcoMart.Data;
using EcoMart.Data.Enum;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingRepository _shippingRepository;
        private readonly ApplicationDbContext _context;

        public OrderController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOrderRepository orderRepository, IShippingRepository shippingRepository, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _orderRepository = orderRepository;
            _shippingRepository = shippingRepository;
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

            //filter by
            OrderStatus? orderStatusFilter = null;
            if (!string.IsNullOrEmpty(filterBy)) {
                if (Enum.TryParse(filterBy, out OrderStatus parsedStatus)) {
                    orderStatusFilter = parsedStatus;
                }
            }

            int termInt;
            bool isTermValidInt = int.TryParse(term, out termInt);
            var orderData = new OrderViewModel();

            var orders = (from order in _context.Orders
                          where (orderStatusFilter == null || order.OrderStatus == orderStatusFilter) && (string.IsNullOrEmpty(term) || order.Id == termInt)
                          orderby (order.OrderStatus == 0 && order.PaymentStatus == 0 ? 1 : 0) descending,
                                   order.OrderStatus,
                                   order.OrderDate descending,
                                   order.PaymentStatus
                          select new Order {
                              Id = order.Id,
                              OrderDate = order.OrderDate,
                              OrderStatus = order.OrderStatus,
                              TotalAmount = order.TotalAmount,
                              PaymentStatus = order.PaymentStatus,
                              ShippingAddress = order.ShippingAddress,
                              CreatedAt = order.CreatedAt,
                              UpdatedAt = order.UpdatedAt,
                          });

            if (!orders.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = orders.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            orders = orders.Skip((currentPage - 1) * pageSize).Take(pageSize);

            orderData.Orders = orders;
            orderData.Term = term;
            orderData.FilterBy = filterBy;
            orderData.CurrentPage = currentPage;
            orderData.TotalPages = totalPage;
            orderData.PageSize = pageSize;
            return View(orderData);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return Json(new { success = false, message = "Order not found" });

            var shipping = await _shippingRepository.GetByOrderIdAsync(id);
            if (shipping == null) return Json(new { success = false, message = "Shipping not found" });

            try {
                order.OrderStatus = OrderStatus.Shipped;
                shipping.ShippedDate = DateTime.Now;
                _orderRepository.Update(order);
                _shippingRepository.Update(shipping);
                return Json(new { success = true });
            } catch {
                return Json(new { success = false, message = "Shipping the order failed!" });
            }
        }
    }
}
