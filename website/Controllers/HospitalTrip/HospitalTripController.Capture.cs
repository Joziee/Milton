using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.HospitalTrip;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.HospitalTrip
{
    public partial class HospitalTripController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _hospitalTripService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Hospital Trip for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;

            var model = result.Select(x => new HospitalTripViewModel()
            {
                Date = x.Date.ToString("dd/MM/yyyy"),
                HospitalName = x.Hospital.Name,
                HospitalTripId = x.HospitalTripId,
                ReturnTrip = x.ReturnTrip
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(HospitalTripViewModel model)
        {
            if (model == null) return RedirectToAction("Index");
            int accountId = model.AccountId;
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<HospitalTripViewModel, Milton.Database.Models.Business.HospitalTrip>();
            Milton.Database.Models.Business.HospitalTrip hospitalTrip = AutoMapper.Mapper.Map<HospitalTripViewModel, Milton.Database.Models.Business.HospitalTrip>(model);

            var ctx = Users.UserContext();

            hospitalTrip.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _hospitalTripService.Insert(hospitalTrip);

                    //Commit
                    scope.Complete();
                }
                catch (DbEntityValidationException valExc)
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Capture", ex);
                    var result = _hospitalTripService.GetByAccountId(accountId);
                    var account = _accountService.GetById(accountId);

                    ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
                    ViewBag.Title = "Capture Hospital Trip for " + account.Name + " " + account.Surname;
                    ViewBag.AccountId = accountId;

                    var returnModel = result.Select(x => new HospitalTripViewModel()
                    {
                        Date = x.Date.ToString("dd/MM/yyyy"),
                        HospitalName = x.Hospital.Name,
                        HospitalTripId = x.HospitalTripId,
                        ReturnTrip = x.ReturnTrip
                    }).ToList();

                    return View(returnModel);
                }
            }

            AddNotification("The hospital trip was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.HospitalTripId = hospitalTrip.HospitalTripId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}