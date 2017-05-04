using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Rates
{
    public partial class RatesController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Hospital Rates";
            return View();
        }
    }
}