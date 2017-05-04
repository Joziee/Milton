using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Report
{
    public partial class ReportController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Report";
            return View();
        }
    }
}
