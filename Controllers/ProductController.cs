using System;
using System.Data.Entity; 
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;
using _2001230507_NhanTuManh_B2.Security;


namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class ProductsController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var product = await db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductOptions)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public ActionResult Create([Bind(Include = "ProductName,Description,Price,CategoryID,ImageURL,IsAvailable")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<ActionResult> Edit([Bind(Include = "ProductID,ProductName,Description,Price,ImageURL,CategoryID,IsAvailable,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.UpdatedAt = DateTime.Now;
                // Đánh dấu đối tượng là đã bị sửa đổi
                db.Entry(product).State = EntityState.Modified;
                // Đảm bảo CreatedAt không bị sửa đổi
                db.Entry(product).Property(p => p.CreatedAt).IsModified = false;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Thêm phương thức Dispose để giải phóng DbContext
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