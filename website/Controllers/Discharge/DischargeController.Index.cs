using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Discharge
{
    public partial class DischargeController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Patient Discharges";
            return View();
        }
    }
}