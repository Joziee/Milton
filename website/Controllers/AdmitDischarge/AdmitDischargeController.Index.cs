using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.AdmitDischarge
{
    public partial class AdmitDischargeController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Patient Admissions / Discharges";
            return View();
        }
    }
}