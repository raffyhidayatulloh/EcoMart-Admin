using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.Services;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EcoMart.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;

        public ProductController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IProductRepository productRepository, ApplicationDbContext context, IPhotoService photoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _productRepository = productRepository;
            _context = context;
            _photoService = photoService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string term="", string filterBy="", int currentPage=1)
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
            var categories = _context.Categories.OrderBy(c => c.CategoryName).ToList();
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            filterBy = string.IsNullOrEmpty(filterBy) ? "" : filterBy;
            var proData = new ProductViewModel();

            var products = (from pro in _context.Products
                            where (term == "" || pro.ProductName.ToLower().Contains(term.ToLower()))
                            && (filterBy == "" || pro.CategoryId == int.Parse(filterBy))
                            orderby pro.ProductName
                            select new Product {
                                Id = pro.Id,
                                ProductName = pro.ProductName,
                                Description = pro.Description,
                                StockQuantity = pro.StockQuantity,
                                ImageUrl = pro.ImageUrl
                            });

            if (!products.Any()) TempData["NoData"] = "No data has been found";

            int totalRecords = products.Count();
            int pageSize = 15;
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);

            proData.Products = products;
            proData.Categories = categories;
            proData.Term = term;
            proData.FilterBy = filterBy;
            proData.CurrentPage = currentPage;
            proData.TotalPages = totalPage;
            proData.PageSize = pageSize;
            return View(proData);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            var productVM = productViewModel.CreateProductVM;
            if (productVM == null) {
                TempData["ErrorMessage"] = "Failed to save the product!";
                return RedirectToAction("Index");
            }

            var result = await _photoService.AddPhotoAsync(productVM.ImageUrl);
            if (result != null) {
                var product = new Product {
                    ProductName = productVM.ProductName,
                    Description = productVM.Description,
                    Price = productVM.Price,
                    StockQuantity = productVM.StockQuantity,
                    CategoryId = productVM.CategoryId,
                    ImageUrl = result.Url.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                _productRepository.Add(product);
                TempData["SuccessMessage"] = "Product saved successfully!";
                return RedirectToAction("Index");
            } else {
                TempData["ErrorMessage"] = "Failed to save the product!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var productData = new GetProductViewModel {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                ImageUrl = product.ImageUrl,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };
            return Json(productData);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel productViewModel)
        {
            var productVM = productViewModel.EditProductVM;
            if (productVM == null) {
                TempData["ErrorMessage"] = "Failed to save the product!";
                return RedirectToAction("Index");
            }

            var editProduct = await _productRepository.GetByIdAsyncNoTracking(id);
            if (editProduct == null) {
                TempData["ErrorMessage"] = "Product not found!";
                return RedirectToAction("Index");
            }

            var photoResult = productVM.ImageUrl;

            if (productVM.Image != null) {
                var publicId = ExtractPublicIdFromUrl(editProduct.ImageUrl);
                if (!string.IsNullOrEmpty(publicId)) {
                    var deletionResult = await _photoService.DeletePhotoAsync(publicId);
                    if (deletionResult.Result == "ok") {
                        var photo = await _photoService.AddPhotoAsync(productVM.Image);
                        photoResult = photo.Url.ToString();
                    } else {
                        TempData["ErrorMessage"] = "Failed to delete photo from Cloudinary!";
                        return RedirectToAction("Index");
                    }
                } else {
                    TempData["ErrorMessage"] = "Invalid publicId!";
                    return RedirectToAction("Index");
                }
            }

            var product = new Product {
                Id = id,
                ProductName = productVM.ProductName,
                Description = productVM.Description,
                Price = productVM.Price,
                StockQuantity = productVM.StockQuantity,
                CategoryId = productVM.CategoryId,
                ImageUrl = photoResult,
                CreatedDate = productVM.CreatedDate,
                UpdatedDate = DateTime.Now
            };

            _productRepository.Update(product);
            TempData["SuccessMessage"] = "Product saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductAjax([FromBody] Product model)
        {
            var productDetails = await _productRepository.GetByIdAsync(model.Id);
            if (productDetails == null) return Json(new { success = false, message = "Product not found" });

            try {
                var publicId = ExtractPublicIdFromUrl(productDetails.ImageUrl);

                if (!string.IsNullOrEmpty(publicId)) {
                    var deletionResult = await _photoService.DeletePhotoAsync(publicId);

                    if (deletionResult.Result == "ok") {
                        _productRepository.Delete(productDetails);
                        return Json(new { success = true });
                    } else {
                        return Json(new { success = false, message = "Failed to delete photo from Cloudinary." });
                    }
                } else {
                    return Json(new { success = false, message = "Invalid publicId." });
                }
            } catch {
                return Json(new { success = false, message = "An error occurred while deleting the photo." });
            }
        }

        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;

            var publicIdWithExtension = segments.Last();
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);
            return publicId;
        }
    }
}
