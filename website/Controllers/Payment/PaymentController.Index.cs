using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Payment
{
    public partial class PaymentController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Payments";
            return View();
        }
    }
}
