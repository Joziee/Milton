using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.GenerateReport
{
    public partial class GenerateReportController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Generate Reports";
            return View();
        }
    }
}