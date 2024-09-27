using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcoMart.Controllers
{
    public class ReviewController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManage;
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public ReviewController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManage, IReviewRepository reviewRepository, IProductRepository productRepository,ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManage = signInManage;
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string term = "", int currentPage = 1)
        {
            // user info
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!string.IsNullOrEmpty(role)) role = char.ToUpper(role[0]) + role.Substring(1).ToLower();
            ViewBag.UserName = userName;
            ViewBag.Role = role;

            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            var revData = new ReviewViewModel();

            var query = from pro in _context.Products
                        join rev in _context.Reviews on pro.Id equals rev.ProductId into reviewsGroup
                        from review in reviewsGroup.DefaultIfEmpty()
                        where pro.ProductName.ToLower().Contains(term)
                        group review by new {
                            pro.Id,
                            pro.ProductName,
                            pro.Category.CategoryName
                        } into grouped
                        select new ProductReviewViewModel {
                            ProductId = grouped.Key.Id,
                            ProductName = grouped.Key.ProductName,
                            CategoryName = grouped.Key.CategoryName,
                            AverageRating = grouped.Average(r => r != null ? r.Rating : 0)
                        };

            query = query.OrderBy(a => a.ProductName);

            int totalRecords = query.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

            revData.Reviews = query;
            revData.CurrentPage = currentPage;
            revData.TotalPages = totalPage;
            revData.PageSize = pageSize;
            return View(revData);
        }

        public async Task<IActionResult> Detail(int id, int currentPage = 1)
        {
            var getProduct = await _productRepository.GetByIdAsync(id);
            if (getProduct != null) {
                var revData = new DetailReviewViewModel();

                var query = _context.Reviews.Where(rev => rev.ProductId == id);

                revData.TotalReviews = query.Count();
                revData.AverageRating = query.Any() ? query.Average(rev => rev.Rating) : 0;

                revData.RatingCount1 = query.Count(rev => rev.Rating == 1);
                revData.RatingCount2 = query.Count(rev => rev.Rating == 2);
                revData.RatingCount3 = query.Count(rev => rev.Rating == 3);
                revData.RatingCount4 = query.Count(rev => rev.Rating == 4);
                revData.RatingCount5 = query.Count(rev => rev.Rating == 5);

                int totalRecords = query.Count();
                int pageSize = 15;
                int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
                query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

                revData.Reviews = query;
                revData.ProductId = id;
                revData.CurrentPage = currentPage;
                revData.TotalPages = totalPage;
                revData.PageSize = pageSize;
                return View(revData);
            }
            return RedirectToAction("Index", "Review");
        }
    }
}