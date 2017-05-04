using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
    {
        public ActionResult Depart()
        {
            ViewBag.Title = "Patient Depart";
            return View();
        }
    }
}