using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Services;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public CategoryController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ICategoryRepository categoryRepository, IProductRepository productRepository,ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string term="", int currentPage = 1)
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
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            var catData = new CategoryViewModel();

            var categories = (from cat in _context.Categories
                              where term == "" || cat.CategoryName.ToLower().Contains(term.ToLower())
                              orderby cat.CategoryName
                              select new CategoryProductCountList {
                                  Id = cat.Id,
                                  CategoryName = cat.CategoryName,
                                  CreatedDate = cat.CreatedDate,
                                  UpdatedDate = cat.UpdatedDate,
                                  ProductCount = _context.Products.Count(p => p.CategoryId == cat.Id)
                              });

            if (!categories.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = categories.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            categories = categories.Skip((currentPage - 1) * pageSize).Take(pageSize);

            catData.Categories = categories;
            catData.Term = term;
            catData.CurrentPage = currentPage;
            catData.TotalPages = totalPage;
            catData.PageSize = pageSize;
            return View(catData);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)  return NotFound();

            var categoryData = new {
                id = category.Id,
                categoryName = category.CategoryName,
                createdDate = category.CreatedDate,
                updatedDate = category.UpdatedDate
            };
            return Json(categoryData);
        }

        [HttpPost]
        public IActionResult Create(CategoryViewModel categoryViewModel)
        {
            var categoryVM = categoryViewModel.CreateCategoryVM;
            if (categoryViewModel == null) {
                TempData["ErrorMessage"] = "Failed to save the category!";
                return RedirectToAction("Index");
            }

            var category = new Category {
                CategoryName = categoryVM.CategoryName,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };
            _categoryRepository.Add(category);
            TempData["SuccessMessage"] = "Category saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryViewModel categoryViewModel)
        {
            var categoryVM = categoryViewModel.EditCategoryVM;
            if (categoryVM == null) {
                TempData["ErrorMessage"] = "Failed to save the category!";
                return RedirectToAction("Index");
            }

            var editCategory = await _categoryRepository.GetByIdAsyncNoTracking(id);
            if (editCategory == null) {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("Index");
            }

            var category = new Category {
                Id = id,
                CategoryName = categoryVM.CategoryName,
                CreatedDate = categoryVM.CreatedDate,
                UpdatedDate = DateTime.Now
            };
            _categoryRepository.Update(category);
            TempData["SuccessMessage"] = "Category saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryAjax([FromBody] Category model)
        {
            var categoryDetails = await _categoryRepository.GetByIdAsync(model.Id);
            if (categoryDetails == null) return NotFound();

            try {
                _categoryRepository.Delete(categoryDetails);
                return Json(new { success = true });
            } catch {
                return NotFound();
            }
        }
    }
}
