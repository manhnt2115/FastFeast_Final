using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;
using System.Linq;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class DeliveryDriversController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: DeliveryDrivers
        public async Task<ActionResult> Index()
        {
            var drivers = await db.DeliveryDrivers
                .Include(d => d.Deliveries)
                .ToListAsync();
            return View(drivers);
        }

        // GET: DeliveryDrivers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Chú ý: ThenInclude() được thay thế
            var driver = await db.DeliveryDrivers
                .Include("Deliveries.Order") // Thay thế cho ThenInclude
                .FirstOrDefaultAsync(m => m.DriverID == id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // GET: DeliveryDrivers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeliveryDrivers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FirstName,LastName,PhoneNumber,Email,VehicleType,LicensePlate,IsAvailable,CurrentLocation")] DeliveryDriver driver)
        {
            if (ModelState.IsValid)
            {
                db.DeliveryDrivers.Add(driver);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // GET: DeliveryDrivers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var driver = await db.DeliveryDrivers.FindAsync(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: DeliveryDrivers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DriverID,FirstName,LastName,PhoneNumber,Email,VehicleType,LicensePlate,IsAvailable,CurrentLocation")] DeliveryDriver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // GET: DeliveryDrivers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var driver = await db.DeliveryDrivers
                .Include(d => d.Deliveries)
                .FirstOrDefaultAsync(m => m.DriverID == id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: DeliveryDrivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var driver = await db.DeliveryDrivers.FindAsync(id);
            if (driver != null)
            {
                db.DeliveryDrivers.Remove(driver);
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