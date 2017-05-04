using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.BorderTrip;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.BorderTrip
{
    public partial class BorderTripController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _borderTripService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Border Trip for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;

            var model = result.Select(x => new BorderTripViewModel()
            {
                Date = x.Date.ToString("dd/MM/yyyy"),
                AccountId = accountId,
                Amount = x.Amount,
                Description = x.Description,
                BorderTripId = x.BorderTripId,
                Name = x.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(BorderTripViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<BorderTripViewModel, Milton.Database.Models.Business.BorderTrip>();
            Milton.Database.Models.Business.BorderTrip borderTrip = AutoMapper.Mapper.Map<BorderTripViewModel, Milton.Database.Models.Business.BorderTrip>(model);

            var ctx = Users.UserContext();

            borderTrip.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _borderTripService.Insert(borderTrip);

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

            AddNotification("The border trip was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.BorderTripId = borderTrip.BorderTripId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}