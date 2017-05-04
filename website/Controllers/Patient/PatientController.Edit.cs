using Milton.Website.Models.Patient;
using System;
using System.Transactions;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
    {
        /// <summary>
		/// A form to edit a patient
		/// </summary>
		/// <param name="patientId"></param>
		/// <returns></returns>
		[HttpGet]
        public ActionResult Edit(Int32 accountId)
        {
            Milton.Database.Models.Business.Account account = _accountService.GetById(accountId);

            PatientViewModel model = new PatientViewModel()
            {
                ArrivalDate = account.ArrivalDate,
                Gaurdian = account.Gaurdian,
                GaurdianIdNumber = account.GaurdianIdNumber,
                GaurdianName = account.GaurdianName,
                GaurdianSurname = account.GaurdianSurname,
                IdNumber = account.IdNumber,
                Name = account.Name,
                //AccountId = account.AccountId,
                RegionId = account.RegionId,
                Surname = account.Surname,
                //DateOfBirth = account.DateOfBirth.HasValue ? patient.DateOfBirth.Value.ToString("MM/dd/yyyy") : "",
                //GaurdianDateOfBirth = account.GaurdianDateOfBirth.HasValue ? patient.GaurdianDateOfBirth.Value.ToString("MM/dd/yyyy") : ""
            };

            ViewBag.Title = "Edit Patient (" + model.Name + " " + model.Surname + ")";

            return View(model);
        }

        /// <summary>
		/// Update the patient
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost, ValidateInput(false)]
        public ActionResult Edit(PatientViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<PatientViewModel, Milton.Database.Models.Medical.Patient>();
            Milton.Database.Models.Medical.Patient patient = AutoMapper.Mapper.Map<PatientViewModel, Milton.Database.Models.Medical.Patient>(model);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //calculate date of birth
                    if (model.RegionId == 1)
                    {
                        string idPart = model.DateOfBirth.Replace("/", "");
                        patient.DateOfBirth = DateTime.ParseExact(idPart, "MMddyyyy", null);

                        if (model.Gaurdian)
                        {
                            string gaurdianIdPart = model.GaurdianDateOfBirth.Replace("/", "");
                            patient.GaurdianDateOfBirth = DateTime.ParseExact(gaurdianIdPart, "MMddyyyy", null);
                        }
                    }
                    else
                    {
                        string idPart = model.IdNumber.Substring(0, 6);
                        patient.DateOfBirth = DateTime.ParseExact(idPart, "yyMMdd", null);

                        if (model.Gaurdian)
                        {
                            string gaurdianIdPart = model.GaurdianIdNumber.Substring(0, 6);
                            patient.GaurdianDateOfBirth = DateTime.ParseExact(gaurdianIdPart, "yyMMdd", null);
                        }
                    }

                    //fix hospitals
                    if (patient.HospitalId2 == 0) patient.HospitalId2 = null;
                    if (patient.HospitalId3 == 0) patient.HospitalId3 = null;

                    //Update
                    _patientService.Update(patient);

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

            AddNotification("Successfully updated patient!", true);

            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}