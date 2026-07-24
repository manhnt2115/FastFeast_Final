using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2001230507_NhanTuManh_B2.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice
        {
            get { return Product.Price * Quantity; }
        }
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; }
        public decimal SubTotal { get; set; }      
        public decimal DiscountAmount { get; set; } 
        public string AppliedPromoCode { get; set; } 
        public decimal GrandTotal { get; set; }     
    }
}