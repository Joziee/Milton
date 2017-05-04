using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.AdmitDischarge;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Milton.Website.Controllers.AdmitDischarge
{
    public partial class AdmitDischargeController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _admitDischargeService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Admission/Discharge for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;

            var model = result.Select(x => new AdmitDischargeViewModel()
            {
                AdmitDischargeId = x.AdmitDischargeId,
                AccountId = x.AccountId,
                AdmittedDate = x.AdmittedDate,
                DischargedDate = x.DischargedDate,
                HospitalId = x.HospitalId,
                HospitalName = x.Hospital.Name,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(AdmitDischargeViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<AdmitDischargeViewModel, Milton.Database.Models.Medical.AdmitDischarge>();
            Milton.Database.Models.Medical.AdmitDischarge admitDischarge = AutoMapper.Mapper.Map<AdmitDischargeViewModel, Milton.Database.Models.Medical.AdmitDischarge>(model);

            var ctx = Users.UserContext();

            admitDischarge.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //get last state (admitted or discharged
                    var admittedDischargedResult = _admitDischargeService.GetByAccountId(model.AccountId);
                    bool discharged = true;
                    if (admittedDischargedResult.Any())
                    {
                        discharged = admittedDischargedResult.LastOrDefault().DischargedDate.HasValue;
                    }

                    if (!discharged)
                    {
                        //Update
                        var lastAdmitDischarged = _admitDischargeService.GetById(admittedDischargedResult.LastOrDefault().AdmitDischargeId);
                        lastAdmitDischarged.DischargedDate = model.DischargedDate;
                        _admitDischargeService.Update(lastAdmitDischarged);
                    }
                    else
                    {
                        //Create
                        _admitDischargeService.Insert(admitDischarge);
                    }

                    //Commit
                    scope.Complete();
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

            AddNotification("The admission/discharge was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.AdmitDischargeId = admitDischarge.AdmitDischargeId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}