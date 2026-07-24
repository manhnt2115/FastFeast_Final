using System.Web;
using System.Web.Mvc;

namespace _2001230507_NhanTuManh_B2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
