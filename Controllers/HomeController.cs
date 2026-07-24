using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using _2001230507_NhanTuManh_B2.Models;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class HomeController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.ToList();
            var products = db.Products.Include(p => p.Category)
                                      .Where(p => p.IsAvailable == true)
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToList();

            return View(products);
        }

        public ActionResult Menu()
        {
            return RedirectToAction("Index", "Products");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Về chúng tôi.";
            return View();
        }
    }
}