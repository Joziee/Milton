using Milton.Services.Cache;
using Milton.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers
{
    [AccessControlFilter]
    public class BaseController : Controller
    {
        public void AddNotification(string message, bool success)
        {
            if (success)
                TempData.Add("NotificationSuccess", message);
            else
                TempData.Add("NotificationError", message);
        }
    }
}
