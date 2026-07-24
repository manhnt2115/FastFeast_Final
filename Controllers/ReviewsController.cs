using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using _2001230507_NhanTuManh_B2.Models;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class ReviewsController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: Reviews
        public async Task<ActionResult> Index()
        {
            var reviews = await db.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.Order)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var review = await db.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.Order)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        // GET: Reviews/Create
        public ActionResult Create()
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            int currentUserID = (int)Session["CustomerID"];
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

            // 3. Lấy danh sách Order CỦA RIÊNG KHÁCH HÀNG ĐÓ
            var myOrders = db.Orders
                             .Where(o => o.CustomerID == currentUserID) // Chỉ lấy đơn của user này
                             .ToList() // Lấy dữ liệu về RAM để xử lý chuỗi bên dưới
                             .Select(o => new
                             {
                                 OrderID = o.OrderID,
                                 DisplayText = "Đơn hàng #" + o.OrderID + " (" + (o.OrderDate.HasValue ? o.OrderDate.Value.ToString("dd/MM/yyyy") : "N/A") + ")"
                             });

            // Tạo SelectList với DisplayText vừa tạo
            ViewBag.OrderID = new SelectList(myOrders, "OrderID", "DisplayText");

            // 4. Lấy thông tin khách hàng (để hiển thị mặc định nếu cần)
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", currentUserID);

            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review)
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            review.CustomerID = (int)Session["CustomerID"];
            review.ReviewDate = DateTime.Now;

            if (review.ProductID == null && review.OrderID == null)
            {
                ModelState.AddModelError("", "Vui lòng chọn Sản phẩm hoặc Đơn hàng để đánh giá.");
            }

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";

                // 4. Điều hướng
                // Nếu review sản phẩm -> Về trang chi tiết sản phẩm
                if (review.ProductID != null)
                {
                    return RedirectToAction("Details", "Products", new { id = review.ProductID });
                }
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", review.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", review.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", review.OrderID);

            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", review.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", review.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", review.OrderID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReviewID,CustomerID,ProductID,OrderID,Rating,Comment,ReviewDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", review.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", review.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", review.OrderID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var review = await db.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Include(r => r.Order)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var review = await db.Reviews.FindAsync(id);
            if (review != null)
            {
                db.Reviews.Remove(review);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
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