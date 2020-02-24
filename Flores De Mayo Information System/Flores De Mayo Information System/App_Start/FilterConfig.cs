using System.Web;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
