using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
    {
        public ActionResult Delete(int patientId)
        {
            var patient = _patientService.GetById(patientId);

            if (patient == null) return RedirectToAction("Index");

            _patientService.Delete(patient);

            return RedirectToAction("Index");
        }
    }
}