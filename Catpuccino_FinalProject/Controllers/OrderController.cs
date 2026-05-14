using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Data;
using Catpuccino_FinalProject.Models;
using Microsoft.EntityFrameworkCore;


namespace Catpuccino_FinalProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Order()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            //  Pull from DB, then map Order → UserOrder (your existing view model)
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .Select(o => new UserOrder
                {
                    Id = o.Id,
                    Date = o.Date.ToString("MM/dd/yyyy"),  // DateTime → string to match UserOrder
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Items = o.Items.Select(i => new CartItemDto  // OrderItem → CartItemDto
                    {
                        Name = i.Name,
                        Size = i.Size,
                        Price = i.Price,
                        Qty = i.Qty
                    }).ToList()
                })
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult PlaceOrder([FromBody] List<CartItemDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
                return BadRequest("Cart is empty");

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            //  Map CartItemDto → OrderItem when saving to DB
            var newOrder = new Order
            {
                Date = DateTime.Now,
                Status = "Pending",
                TotalAmount = cartItems.Sum(item => item.Price * item.Qty),
                UserId = userId,
                Items = cartItems.Select(i => new OrderItem
                {
                    Name = i.Name,
                    Size = i.Size,
                    Price = i.Price,
                    Qty = i.Qty
                }).ToList()
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return Json(new { success = true, redirectUrl = Url.Action("Order", "Order") });
        }
    }
}