using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Recon
{
    public partial class ReconController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Recon";
            return View();
        }
    }
}
