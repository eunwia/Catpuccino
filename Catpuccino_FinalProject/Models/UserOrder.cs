using System;
using System.Collections.Generic;

namespace Catpuccino_FinalProject.Models
{
    //  DATABASE entity (saved to SQL)
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; } = "";
        public string Size { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public Order Order { get; set; } = null!;
    }

    //  VIEW MODEL (used by Order History page)
    public class UserOrder
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    //  Maps directly to the cart array items in JavaScript
    public class CartItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}