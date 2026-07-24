using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;
using System.Collections.Generic;


namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class OrdersController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: Orders
        public async Task<ActionResult> Index(string status, DateTime? fromDate, DateTime? toDate)
        {
            var orders = db.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.OrderStatus == status);
            }
            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= toDate.Value);
            }

            return View(await orders.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders
                .Include(o => o.Customer)
                .Include("OrderDetails.Product")
                .Include(o => o.Deliveries)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CustomerID,TotalAmount,OrderStatus,ShippingAddress,ShippingCity,ShippingPostalCode,PaymentMethod,PaymentStatus,DeliveryFee,DiscountAmount,EstimatedDeliveryTime,Notes")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderID,CustomerID,OrderDate,TotalAmount,OrderStatus,ShippingAddress,ShippingCity,ShippingPostalCode,PaymentMethod,PaymentStatus,DeliveryFee,DiscountAmount,EstimatedDeliveryTime,ActualDeliveryTime,Notes")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var order = await db.Orders.FindAsync(id);
            if (order != null)
            {
                db.Orders.Remove(order);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        // GET: Orders/Checkout
        public ActionResult Checkout()
        {
            // 1. Kiểm tra đăng nhập
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Kiểm tra giỏ hàng
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // 3. Lấy thông tin khách hàng để điền sẵn vào form
            int customerId = (int)Session["CustomerID"];
            var customer = db.Customers.Find(customerId);

            // 4. Tính toán tiền nong
            decimal subTotal = cart.Sum(x => x.TotalPrice);
            decimal discount = Convert.ToDecimal(Session["DiscountAmount"] ?? 0);
            decimal total = subTotal - discount;
            if (total < 0) total = 0;

            // Truyền dữ liệu qua ViewBag để hiển thị
            ViewBag.CartItems = cart;
            ViewBag.SubTotal = subTotal;
            ViewBag.Discount = discount;
            ViewBag.TotalAmount = total;

            // Tạo một Order model rỗng để điền thông tin giao hàng
            var orderModel = new Order
            {
                ShippingAddress = customer.Address,
                ShippingCity = customer.City,
                ShippingPostalCode = customer.PostalCode,
                // Gán tạm tên người nhận vào ViewBag hoặc tạo property riêng nếu Order có
            };

            // Lưu ý: Nếu bảng Order chưa có cột ReceiverName/Phone, 
            // bạn có thể dùng tạm ShippingAddress để lưu hoặc phải sửa DB.

            return View(orderModel);
        }

        // POST: Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(Order order, string PaymentMethod)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            if (Session["CustomerID"] == null) return RedirectToAction("Login", "Account");

            // 1. Hoàn thiện thông tin Order
            order.CustomerID = (int)Session["CustomerID"];
            order.OrderDate = DateTime.Now;

            // Tính lại tổng tiền (để bảo mật, tránh user sửa html)
            decimal subTotal = cart.Sum(x => x.TotalPrice);
            decimal discount = Convert.ToDecimal(Session["DiscountAmount"] ?? 0);
            order.TotalAmount = subTotal - discount;
            if (order.TotalAmount < 0) order.TotalAmount = 0;

            order.OrderStatus = "Pending"; // Mới đặt -> Chờ xử lý
            order.PaymentMethod = PaymentMethod; // Cash hoặc Transfer
            order.PaymentStatus = "Unpaid";

            // Nếu chuyển khoản, có thể set PaymentStatus = "Pending Verification"

            try
            {
                // 2. Lưu Order
                db.Orders.Add(order);
                db.SaveChanges(); // Để lấy OrderID

                // 3. Lưu OrderDetails
                foreach (var item in cart)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.Product.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        Subtotal = item.TotalPrice
                    };
                    db.OrderDetails.Add(detail);
                }
                db.SaveChanges();

                // 4. Xóa giỏ hàng và Session khuyến mãi
                Session["Cart"] = null;
                Session["PromoCode"] = null;
                Session["DiscountAmount"] = 0;

                // 5. Chuyển hướng đến trang thông báo thành công
                // Bạn cần tạo View: Views/Orders/OrderSuccess.cshtml
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi xử lý đơn hàng: " + ex.Message);
                // Load lại dữ liệu để hiện lại View
                ViewBag.CartItems = cart;
                ViewBag.SubTotal = subTotal;
                ViewBag.Discount = discount;
                ViewBag.TotalAmount = order.TotalAmount;
                return View(order);
            }
        }

        // Trang thông báo thành công
        public ActionResult OrderSuccess(int id)
        {
            return View(id);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
