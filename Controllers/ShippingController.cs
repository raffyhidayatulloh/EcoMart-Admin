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
    public class ShippingController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IShippingRepository _shippingRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public ShippingController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IShippingRepository shippingRepository, IOrderRepository orderRepository,ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _shippingRepository = shippingRepository;
            _orderRepository = orderRepository;
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
            string filterDelivered = null;
            int filterValue;
            if (int.TryParse(filterBy, out filterValue) && filterValue == 1) {
                filterDelivered = "1";
            }

            int termInt;
            bool isTermValidInt = int.TryParse(term, out termInt);
            var shipData = new ShippingViewModel();

            var shippings = (from ship in _context.Shippings
                             where ((filterDelivered == null && ship.DeliveredDate == null) || (filterDelivered == "1" && ship.DeliveredDate != null) || (string.IsNullOrEmpty(filterBy))) && (string.IsNullOrEmpty(term) || ship.OrderId == termInt)
                             orderby
                                (ship.ShippedDate != null && ship.DeliveredDate == null ? 0 : 1),
                                (ship.ShippedDate == null && ship.DeliveredDate == null ? 0 : 1),
                                (ship.ShippedDate != null && ship.DeliveredDate != null ? 0 : 1),
                                ship.DeliveredDate descending
                             select new Shipping {
                                 Id = ship.Id,
                                 OrderId = ship.OrderId,
                                 ShippingCost = ship.ShippingCost,
                                 ShippedDate = ship.ShippedDate,
                                 DeliveredDate = ship.DeliveredDate,
                                 CreatedAt = ship.CreatedAt,
                                 UpdatedAt = ship.UpdatedAt
                             });

            if (!shippings.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = shippings.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            shippings = shippings.Skip((currentPage - 1) * pageSize).Take(pageSize);

            shipData.Shippings = shippings;
            shipData.Term = term;
            shipData.FilterBy = filterBy;
            shipData.FilterBy = filterBy;
            shipData.CurrentPage = currentPage;
            shipData.TotalPages = totalPage;
            shipData.PageSize = pageSize;
            return View(shipData);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeliveredDate([FromBody] Shipping request)
        {
            if (ModelState.IsValid) {
                var shipping = await _shippingRepository.GetByIdAsync(request.Id);
                if (shipping == null) return Json(new { success = false, message = "Shipping not found" });

                var order = await _orderRepository.GetByIdAsync(shipping.OrderId);
                if (order == null) return Json(new { success = false, message = "Order not found" });

                order.OrderStatus = OrderStatus.Delivered;
                shipping.DeliveredDate = DateTime.Now;
                _shippingRepository.Update(shipping);
                _orderRepository.Update(order);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Failed to deliver the shipping" });
        }
    }
}
