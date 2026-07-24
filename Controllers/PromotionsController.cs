using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;
using System.Linq;
using _2001230507_NhanTuManh_B2.Security;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class PromotionsController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: Promotions
        public async Task<ActionResult> Index()
        {
            return View(await db.Promotions.ToListAsync());
        }

        // GET: Promotions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var promotion = await db.Promotions.FirstOrDefaultAsync(m => m.PromotionID == id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // GET: Promotions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Promotions/Create
        // [QUAN TRỌNG] Phải có [HttpPost] để nhận dữ liệu từ form
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<ActionResult> Create([Bind(Include = "PromotionCode,Description,DiscountType,DiscountValue,MinimumOrderAmount,StartDate,EndDate,UsageLimit,IsActive")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = await db.Promotions.AnyAsync(x => x.PromotionCode == promotion.PromotionCode);

                if (isDuplicate)
                {
                    ModelState.AddModelError("PromotionCode", "Mã khuyến mãi này đã tồn tại. Vui lòng đặt tên khác.");
                    return View(promotion);
                }

                promotion.UsedCount = 0; // Mặc định số lần dùng là 0
                db.Promotions.Add(promotion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var promotion = await db.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<ActionResult> Edit([Bind(Include = "PromotionID,PromotionCode,Description,DiscountType,DiscountValue,MinimumOrderAmount,StartDate,EndDate,UsageLimit,UsedCount,IsActive")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                // [NÂNG CẤP] Kiểm tra trùng lặp khi Sửa (Edit)
                // Logic: Kiểm tra xem có mã nào trùng tên KHÔNG PHẢI là chính nó (PromotionID != promotion.PromotionID)
                bool isDuplicate = await db.Promotions.AnyAsync(x => x.PromotionCode == promotion.PromotionCode && x.PromotionID != promotion.PromotionID);

                if (isDuplicate)
                {
                    ModelState.AddModelError("PromotionCode", "Mã khuyến mãi này đã được sử dụng bởi chương trình khác.");
                    return View(promotion);
                }

                db.Entry(promotion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var promotion = await db.Promotions.FirstOrDefaultAsync(m => m.PromotionID == id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var promotion = await db.Promotions.FindAsync(id);
            if (promotion != null)
            {
                db.Promotions.Remove(promotion);
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