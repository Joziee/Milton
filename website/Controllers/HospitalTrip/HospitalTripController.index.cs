using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.HospitalTrip
{
    public partial class HospitalTripController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Hospital Trip";
            return View();
        }
    }
}