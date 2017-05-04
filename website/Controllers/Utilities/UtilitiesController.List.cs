using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Utilities
{
    public partial class UtilitiesController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Utilities";
            return View();
        }
    }
}
