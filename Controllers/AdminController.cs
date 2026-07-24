using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using _2001230507_NhanTuManh_B2.Models;
using System.Security.Cryptography;
using System.Text;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class AdminController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        public ActionResult Login()
        {
            if (Session["AdminID"] != null) return RedirectToAction("Index", "Products");
            return View();
        }

        // Hàm mã hóa MD5 (Giống bên AccountController)
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
        // POST: Xử lý đăng nhập Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Mã hóa mật khẩu MD5 (để so sánh với DB)
                string passwordHash = GetMD5(model.Password);

                // 2. Tìm tài khoản trong bảng Admins
                var adminAccount = db.Admins.FirstOrDefault(a => a.Username == model.Username && a.PasswordHash == passwordHash);

                if (adminAccount != null)
                {
                    // 3. Đăng nhập thành công -> Lưu Session
                    Session["AdminID"] = adminAccount.AdminID;
                    Session["AdminUser"] = adminAccount.Username;
                    Session["AdminRole"] = adminAccount.Role;

                    // Xóa Session của khách hàng (nếu đang đăng nhập) để tránh xung đột
                    Session["CustomerID"] = null;
                    Session["FullName"] = null;

                    // 4. Chuyển hướng vào trang quản lý
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }


        // Đăng xuất Admin
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        // GET: Admins
        public async Task<ActionResult> Index()
        {
            return View(await db.Admins.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var admin = await db.Admins.FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Username,PasswordHash,Email,Role")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra xem user đã tồn tại chưa
                var checkUser = db.Admins.FirstOrDefault(a => a.Username == admin.Username);
                if (checkUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(admin);
                }

                // 2. MÃ HÓA MẬT KHẨU TRƯỚC KHI LƯU (Code cũ của bạn thiếu dòng này)
                // Lưu ý: Lúc nhập trên form, biến admin.PasswordHash đang chứa mật khẩu thô (VD: 123456)
                admin.PasswordHash = GetMD5(admin.PasswordHash);

                admin.CreatedAt = DateTime.Now;

                db.Admins.Add(admin);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var admin = await db.Admins.FindAsync(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AdminID,Username,PasswordHash,Email,Role,CreatedAt,LastLogin")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var admin = await db.Admins.FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var admin = await db.Admins.FindAsync(id);
            if (admin != null)
            {
                db.Admins.Remove(admin);
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