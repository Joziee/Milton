using Milton.Services.Cache;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Dashboard
{
	public partial class DashboardController : BaseController
	{
        [HttpPost]
        public ActionResult ClearCache()
        {
            var cacheManager = DependencyResolver.Current.GetService<ICacheManager>();
            cacheManager.Clear();

            return Content("");
        }
    }
}
