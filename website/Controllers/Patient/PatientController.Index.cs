using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Capture";
            return View();
        }
    }
}