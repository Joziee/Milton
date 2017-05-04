using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Batch
{
    public partial class BatchController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Batches";
            return View();
        }
    }
}