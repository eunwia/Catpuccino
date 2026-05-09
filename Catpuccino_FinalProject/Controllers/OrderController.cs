using Microsoft.AspNetCore.Mvc;
using Catpuccino_FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Catpuccino_FinalProject.Controllers
{
    public class OrderController : Controller
    {
        // Temporary mock database to store orders while app is running
        private static List<UserOrder> _mockOrders = new List<UserOrder>();

        public IActionResult Index()
        {
            return View();
        }

        // View for the Order History page
        public IActionResult Order()
        {
            // Pass the mock database to the view, ordering by newest first
            return View(_mockOrders.OrderByDescending(o => o.Id).ToList());
        }

        // This receives the cart from JS when "Place Order" is clicked
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] List<CartItemDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Cart is empty");
            }

            // Create a new order record
            var newOrder = new UserOrder
            {
                Id = _mockOrders.Count + 1,
                Date = DateTime.Now.ToString("MM/dd/yyyy"),
                Status = "Pending", // Default status
                TotalAmount = cartItems.Sum(item => item.Price * item.Qty),
                Items = cartItems
            };

            // Save to mock database
            _mockOrders.Add(newOrder);

            // Tell JS it was successful and where to redirect
            return Json(new { success = true, redirectUrl = Url.Action("Order", "Order") });
        }
    }
}