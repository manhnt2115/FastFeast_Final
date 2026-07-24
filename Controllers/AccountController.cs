using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using _2001230507_NhanTuManh_B2.Models;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class AccountController : Controller
    {
        private FastFeastDbContext db = new FastFeastDbContext();

        // GET: Đăng ký
        public ActionResult Register()
        {
            return View();
        }

        // POST: Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra Email đã tồn tại chưa
                var checkEmail = db.Customers.FirstOrDefault(s => s.Email == model.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // 2. Tạo đối tượng Customer mới
                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = GetMD5(model.Password), // Mã hóa mật khẩu
                    RegistrationDate = DateTime.Now
                };

                // 3. Lưu vào database
                db.Customers.Add(customer);
                db.SaveChanges();

                // 4. Chuyển hướng sang trang đăng nhập
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        // POST: Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(model.Password);
                // Kiểm tra Email và Mật khẩu (đã mã hóa)
                var data = db.Customers.FirstOrDefault(s => s.Email.Equals(model.Email) && s.PasswordHash.Equals(f_password));

                if (data != null)
                {
                    // Lưu thông tin vào Session
                    Session["CustomerID"] = data.CustomerID;
                    Session["FullName"] = data.FirstName + " " + data.LastName;
                    Session["UserEmail"] = data.Email;

                    return RedirectToAction("Index", "Home"); // Chuyển về trang chủ
                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng";
                    return View(model);
                }
            }
            return View(model);
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa hết session
            return RedirectToAction("Login");
        }

        // Hàm mã hóa MD5 đơn giản
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
    }
}