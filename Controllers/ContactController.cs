using _2001230507_NhanTuManh_B2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _2001230507_NhanTuManh_B2.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index()
        {
            return View("Contact", new ContactModel());
        }

        [HttpPost]
        public ActionResult Index(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Cảm ơn bạn đã gửi góp ý! Chúng tôi sẽ phản hồi sớm nhất.";
                return View("Success");
            }
            return View("Contact", model);
        }

        public ActionResult Success()
        {
            return RedirectToAction("Success");
        }
    }
}
