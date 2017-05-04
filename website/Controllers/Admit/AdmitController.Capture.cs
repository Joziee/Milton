using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Admit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Admit
{
    public partial class AdmitController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _admitService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Admission for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;

            var model = result.Select(x => new AdmitViewModel()
            {
                AdmitId = x.AdmitId,
                AccountId = x.AccountId,
                AdmittedDate = x.AdmittedDate,
                HospitalId = x.HospitalId,
                HospitalName = x.Hospital.Name,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient,
                DoctorName = x.DoctorName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(AdmitViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<AdmitViewModel, Milton.Database.Models.Medical.Admit>();
            Milton.Database.Models.Medical.Admit admit = AutoMapper.Mapper.Map<AdmitViewModel, Milton.Database.Models.Medical.Admit>(model);

            var ctx = Users.UserContext();

            admit.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _admitService.Insert(admit);

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

            AddNotification("The admission was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.AdmitId = admit.AdmitId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}