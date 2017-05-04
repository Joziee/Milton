using Milton.Website.Models.Admit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Admit
{
    public partial class AdmitController
    {
        /// <summary>
		/// A form to edit a patient
		/// </summary>
		/// <param name="patientId"></param>
		/// <returns></returns>
		[HttpGet]
        public ActionResult Edit(Int32 admitId)
        {
            Milton.Database.Models.Medical.Admit admit = _admitService.GetById(admitId);

            ViewBag.AccountId = admit.AccountId;

            AdmitViewModel model = new AdmitViewModel()
            {
                AdmitId = admit.AdmitId,
                AccountId = admit.AccountId,
                AdmittedDate = admit.AdmittedDate,
                HospitalId = admit.HospitalId,
                HospitalName = admit.Hospital.Name,
                Gaurdian = admit.Gaurdian,
                Patient = admit.Patient,
                DoctorName = admit.DoctorName
            };

            ViewBag.Title = "Edit Admission for (" + admit.Account.Name + " " + admit.Account.Surname + ")";

            return View(model);
        }

        /// <summary>
		/// Update the patient
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost, ValidateInput(false)]
        public ActionResult Edit(AdmitViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<AdmitViewModel, Milton.Database.Models.Medical.Admit>();
            Milton.Database.Models.Medical.Admit admit = AutoMapper.Mapper.Map<AdmitViewModel, Milton.Database.Models.Medical.Admit>(model);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Update
                    _admitService.Update(admit);

                    //Commit
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("model error", ex);
                    //Error handeled by client
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }

            }

            AddNotification("Successfully updated admission!", true);

            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Capture", new { accountId = model.AccountId });

            //Return Json
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}