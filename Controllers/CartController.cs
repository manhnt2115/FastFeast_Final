using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class CartController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // 1. Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            var viewModel = new CartViewModel
            {
                Items = cart,
                SubTotal = cart.Sum(x => x.TotalPrice),
                AppliedPromoCode = Session["PromoCode"] as string,
                DiscountAmount = Convert.ToDecimal(Session["DiscountAmount"] ?? 0)
            };

            // Tính tổng cuối cùng (không âm)
            viewModel.GrandTotal = viewModel.SubTotal - viewModel.DiscountAmount;
            if (viewModel.GrandTotal < 0) viewModel.GrandTotal = 0;

            return View(viewModel);
        }

        // 2. Thêm vào giỏ hàng
        public ActionResult AddToCart(int productId)
        {
            var product = db.Products.Find(productId);
            if (product != null)
            {
                var cart = GetCart();
                var existingItem = cart.FirstOrDefault(x => x.Product.ProductID == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity++; // Đã có thì tăng số lượng
                }
                else
                {
                    cart.Add(new CartItem { Product = product, Quantity = 1 }); // Chưa có thì thêm mới
                }
                Session["Cart"] = cart; // Lưu lại
            }
            return RedirectToAction("Index");
        }

        // 3. Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == productId);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    cart.Remove(item); // Nếu nhập 0 thì xóa luôn
            }

            // Mỗi khi update giỏ hàng, nên reset khuyến mãi để tính lại cho chính xác
            Session["PromoCode"] = null;
            Session["DiscountAmount"] = 0;

            return RedirectToAction("Index");
        }

        // 4. Xóa sản phẩm
        public ActionResult Remove(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProductID == productId);
            if (item != null)
            {
                cart.Remove(item);
            }

            // Reset khuyến mãi
            Session["PromoCode"] = null;
            Session["DiscountAmount"] = 0;

            return RedirectToAction("Index");
        }

        // 5. Áp dụng mã khuyến mãi
        [HttpPost]
        public ActionResult ApplyPromotion(string promoCode)
        {
            // Reset trước
            Session["PromoCode"] = null;
            Session["DiscountAmount"] = 0;

            if (string.IsNullOrEmpty(promoCode))
            {
                return RedirectToAction("Index");
            }

            var cart = GetCart();
            decimal subTotal = cart.Sum(x => x.TotalPrice);

            // Tìm mã trong DB
            var promotion = db.Promotions.FirstOrDefault(p => p.PromotionCode == promoCode && p.IsActive == true);

            if (promotion == null)
            {
                TempData["PromoError"] = "Mã khuyến mãi không tồn tại hoặc đã bị khóa.";
                return RedirectToAction("Index");
            }

            // Kiểm tra ngày hết hạn
            if (promotion.EndDate < DateTime.Now)
            {
                TempData["PromoError"] = "Mã khuyến mãi đã hết hạn.";
                return RedirectToAction("Index");
            }

            // Kiểm tra số lượng dùng
            if (promotion.UsageLimit.HasValue && promotion.UsedCount >= promotion.UsageLimit)
            {
                TempData["PromoError"] = "Mã khuyến mãi đã hết lượt sử dụng.";
                return RedirectToAction("Index");
            }

            // Kiểm tra đơn tối thiểu
            if (subTotal < promotion.MinimumOrderAmount)
            {
                TempData["PromoError"] = $"Đơn hàng phải từ {promotion.MinimumOrderAmount:N0} VNĐ để dùng mã này.";
                return RedirectToAction("Index");
            }

            // Tính toán giảm giá
            decimal discount = 0;
            if (promotion.DiscountType == "Percentage")
            {
                discount = subTotal * (promotion.DiscountValue / 100);
            }
            else // FixedAmount
            {
                discount = promotion.DiscountValue;
            }

            // Lưu vào session
            Session["PromoCode"] = promoCode;
            Session["DiscountAmount"] = discount;
            TempData["PromoSuccess"] = "Áp dụng mã giảm giá thành công!";

            return RedirectToAction("Index");
        }
    }
}