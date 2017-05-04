using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Admit
{
    public partial class AdmitController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Patient Admissions";
            return View();
        }
    }
}