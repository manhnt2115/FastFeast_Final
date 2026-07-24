using System.Web;
using System.Web.Mvc;

namespace _2001230507_NhanTuManh_B2.Security
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra xem Session Admin có tồn tại không
            var adminSession = HttpContext.Current.Session["AdminID"];

            if (adminSession == null)
            {
                // Nếu không phải Admin -> Đá về trang đăng nhập Admin
                filterContext.Result = new RedirectResult("~/Admin/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}