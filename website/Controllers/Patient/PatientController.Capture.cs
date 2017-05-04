using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Patient;
using System;
using System.Data.Entity.Validation;
using System.Transactions;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
    {
        public ActionResult Capture()
        {
            ViewBag.Title = "Capture New Patient";
            return View();
        }

        [HttpPost]
        public ActionResult Capture(PatientViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<PatientViewModel, Milton.Database.Models.Medical.Patient>();
            Milton.Database.Models.Medical.Patient patient = AutoMapper.Mapper.Map<PatientViewModel, Milton.Database.Models.Medical.Patient>(model);

            var ctx = Users.UserContext();

            patient.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var existing = _patientService.GetByCriteria(model.IdNumber);
                    if (existing != null)
                    {
                        ModelState.AddModelError("Capture", "This patient is already captured!");
                    }
                    else
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

                        //Create
                        _patientService.Insert(patient);

                        //Commit
                        scope.Complete();
                    }
                }
                catch (DbEntityValidationException valExc)
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Capture", ex);
                    return View(model);
                }
            }

            AddNotification("The patient was successfully created!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.PatientId = patient.PatientId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}