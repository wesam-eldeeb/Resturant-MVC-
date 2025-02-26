﻿using OnlineRestaurant.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRestaurant.ViewModels
{
    public class OrderItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
