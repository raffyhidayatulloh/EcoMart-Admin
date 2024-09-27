using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace EcoMart.Controllers
{
    public class CouponController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICouponRepository _couponRepository;
        private readonly ApplicationDbContext _context;

        public CouponController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICouponRepository couponRepository, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _couponRepository = couponRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string filterBy = "", int currentPage = 1)
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
            bool isFilterApplied = filterBy == "1" ? true : false;
            var couData = new CouponViewModel();

            var coupons = (from cou in _context.Coupons
                           where filterBy == "" ||
                           (filterBy == "0" && (cou.IsActive == false || cou.ExpiryDate < DateTime.Now)) ||
                           (filterBy == "1" && (cou.IsActive == true || cou.ExpiryDate > DateTime.Now))
                           orderby cou.IsActive descending, cou.ExpiryDate
                           select new Coupon {
                               Id = cou.Id,
                               DiscountPersent = cou.DiscountPersent,
                               MaxDiscount = cou.MaxDiscount,
                               MinPurchase = cou.MinPurchase,
                               ExpiryDate = cou.ExpiryDate,
                               IsActive = cou.IsActive
                           });

            if (!coupons.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = coupons.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            coupons = coupons.Skip((currentPage - 1) * pageSize).Take(pageSize);

            couData.Coupons = coupons;
            couData.FilterBy = filterBy;
            couData.CurrentPage = currentPage;
            couData.TotalPages = totalPage;
            couData.PageSize = pageSize;
            return View(couData);
        }

        [HttpPost]
        public IActionResult Create(CouponViewModel couponViewModel)
        {
            var couponVM = couponViewModel.CreateCouponVM;
            if (couponVM == null) {
                TempData["ErrorMessage"] = "Failed to save the coupon!";
                return RedirectToAction("Index");
            }

            var coupon = new Coupon {
                MaxDiscount = couponVM.MaxDiscount,
                DiscountPersent = couponVM.DiscountPersent,
                MinPurchase = couponVM.MinPurchase,
                ExpiryDate = couponVM.ExpiryDate,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _couponRepository.Add(coupon);
            TempData["SuccessMessage"] = "Coupon saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCouponAjax([FromBody] Coupon model)
        {
            var couponDetails = await _couponRepository.GetByIdAsync(model.Id);
            if (couponDetails == null) return Json(new { success = false, message = "Coupon not found." });

            try {
                _couponRepository.Delete(couponDetails);
                return Json(new { success = true, message = "The coupon has been deleted." });
            } catch {
                return Json(new { success = false, message = "An error occurred while deleting the coupon." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] Coupon model)
        {
            var couponDetails = await _couponRepository.GetByIdAsync(model.Id);
            if (couponDetails == null) return Json(new { success = false, message = "Coupon not found." });

            try {
                couponDetails.IsActive = model.IsActive;
                _couponRepository.Update(couponDetails);
                return Json(new { success = true });
            } catch {
                return Json(new { success = false, message = "An error occurred while updating the coupon." });
            }
        }
    }
}
