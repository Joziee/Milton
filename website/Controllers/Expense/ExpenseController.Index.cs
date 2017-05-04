using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Expense
{
    public partial class ExpenseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Other Expenses";
            return View();
        }
    }
}