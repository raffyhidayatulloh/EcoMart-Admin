using EcoMart.Data;
using EcoMart.Interfaces;
using EcoMart.Models;
using EcoMart.Repository;
using EcoMart.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMart.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public OrderDetailController(IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository, IProductRepository productRepository,ApplicationDbContext context)
        {
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrderDetail(int id, int page = 1, int pageSize = 15)
        {
            var details = _context.OrderDetails.Where(od => od.OrderId == id)
                          .Select(od => new OrderDetail {
                              OrderId = od.OrderId,
                              Product = od.Product,
                              Quantity = od.Quantity,
                              Price = od.Price,
                              SubTotal = od.Quantity * od.Price
                          }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var totalItems = _context.OrderDetails.Count(od => od.OrderId == id);

            var viewModel = new OrderDetailViewModel {
                Details = details,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };
            return Json(viewModel);
        }
    }
}
