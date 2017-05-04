using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.BorderTrip
{
    public partial class BorderTripController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Border Trip";
            return View();
        }
    }
}