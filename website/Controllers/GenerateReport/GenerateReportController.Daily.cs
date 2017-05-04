using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.GenerateReport
{
    public partial class GenerateReportController
    {
        public ActionResult Daily()
        {
            ViewBag.Title = "Daily Report";
            return View();
        }

        [HttpPost]
        public ActionResult DailyBotswana(DateTime date)
        {
            var result = _reportService.GenerateDailyPatients(date, 1);

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Concordia Lodge Patients Botswana - " + date.ToString("dd MM yyyy") + ".xlsx");
        }

        [HttpPost]
        public ActionResult DailySwaziland(DateTime date)
        {
            var result = _reportService.GenerateDailyPatients(date, 2);

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Concordia Lodge Patients Swaziland - " + date.ToString("dd MM yyyy") + ".xlsx");
        }
    }
}