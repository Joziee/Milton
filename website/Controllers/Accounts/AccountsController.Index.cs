using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Accounts
{
    public partial class AccountsController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Patients";
            return View();
        }
    }
}