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
        public ActionResult Edit(int borderTripId)
        {
            var result = _borderTripService.GetById(borderTripId);
            var account = _accountService.GetById(result.AccountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Edit Border Trip for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = account.AccountId;
            ViewBag.BorderTripId = "BT" + borderTripId.ToString().PadLeft(6, '0');

            var model = new BorderTripViewModel()
            {
                Date = result.Date.ToString("MM/dd/yyyy"),
                AccountId = account.AccountId,
                Amount = result.Amount,
                Description = result.Description,
                BorderTripId = result.BorderTripId,
                Name = result.Name
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BorderTripViewModel model)
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
                    _borderTripService.Update(borderTrip);

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

            AddNotification("The border trip was successfully updated!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.BorderTripId = borderTrip.BorderTripId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}