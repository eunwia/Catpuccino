using System;
using System.Collections.Generic;

namespace Catpuccino_FinalProject.Models
{
    public class UserOrder
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Holds the list of items in that specific order
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    // This maps directly to the cart array items in Javascript
    public class CartItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}