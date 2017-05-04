using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Discharge;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Discharge
{
    public partial class DischargeController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _dischargeService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Discharge for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;
            ViewBag.IdNumber = account.IdNumber;

            var model = result.Select(x => new DischargeViewModel()
            {
                DischargeId = x.DischargeId,
                AccountId = x.AccountId,
                DischargeDate = x.DischargeDate,
                HospitalId = x.HospitalId,
                HospitalName = x.Hospital.Name,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(DischargeViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<DischargeViewModel, Milton.Database.Models.Medical.Discharge>();
            Milton.Database.Models.Medical.Discharge discharge = AutoMapper.Mapper.Map<DischargeViewModel, Milton.Database.Models.Medical.Discharge>(model);

            var ctx = Users.UserContext();

            discharge.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _dischargeService.Insert(discharge);

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

            AddNotification("The discharge was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.DischargeId = discharge.DischargeId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateId(DischargeViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<DischargeViewModel, Milton.Database.Models.Medical.Discharge>();
            Milton.Database.Models.Medical.Discharge discharge = AutoMapper.Mapper.Map<DischargeViewModel, Milton.Database.Models.Medical.Discharge>(model);

            var ctx = Users.UserContext();

            discharge.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _dischargeService.Insert(discharge);

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

            AddNotification("The discharge was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.DischargeId = discharge.DischargeId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}