using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.GenerateReport
{
    public partial class GenerateReportController
    {
        public ActionResult Patient()
        {
            ViewBag.Title = "Patient Report";
            return View();
        }

        [HttpPost]
        public ActionResult Patient(int accountId)
        {
            var account = _accountService.GetById(accountId);
            string part = "BOTS";
            if (account.RegionId == 2) part = "SWAZ";
            string accountName = part + account.AccountId.ToString().PadLeft(6, '0');

            var result = _reportService.GeneratePatientReport(accountId);

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Concordia Lodge Patient Report (" + accountName + ") - " + account.Name + " " + account.Surname + ".xlsx");
        }
    }
}