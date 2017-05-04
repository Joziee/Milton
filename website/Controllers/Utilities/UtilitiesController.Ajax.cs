using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Utilities
{
    public partial class UtilitiesController
    {
        [HttpPost]
        public ActionResult Update(int id, DateTime dob, DateTime? gdob = null)
        {
            var patient = _patientService.GetById(id);

            if (patient == null) return Json(new { Success = false, Message = "Patient not found!!" });

            patient.DateOfBirth = dob;

            if (gdob != null) patient.GaurdianDateOfBirth = gdob;

            _patientService.Update(patient);

            return Json(new { Success = true });
        }

        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            var patients = _patientService.GetAll();
            string searchTerm = term.ToLower();

            //filter

            var result = patients.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Surname.ToLower().Contains(searchTerm.ToLower()) || x.IdNumber.ToLower().Contains(searchTerm.ToLower()));

            return Json(result.Select(x =>
            new
            {
                id = x.PatientId,
                label = x.IdNumber + " (" + x.Name + " " + x.Surname + ") - " + x.ArrivalDate.ToString("dd/MM/yyyy"),
                value = x.IdNumber + " (" + x.Name + " " + x.Surname + ") - " + x.ArrivalDate.ToString("dd/MM/yyyy")
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
