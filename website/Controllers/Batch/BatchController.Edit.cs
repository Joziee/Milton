using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Batch;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Batch
{
    public partial class BatchController
    {
        public ActionResult Edit(int? batchId)
        {
            if (!batchId.HasValue) return RedirectToAction("Index");

            ViewBag.Title = "Create a Batch";

            var batch = _batchService.GetById(batchId.Value);
            AutoMapper.Mapper.CreateMap<Milton.Database.Models.Business.Batch, BatchViewModel>();
            BatchViewModel model = AutoMapper.Mapper.Map<Milton.Database.Models.Business.Batch, BatchViewModel>(batch);

            model.AccountIds = String.Join(",", batch.Accounts.Select(a => a.AccountId));
            if (batch.BorderTrips != null)
                model.BorderTripIds = String.Join(",", batch.BorderTrips.Select(x => x.BorderTripId));
            else
                model.BorderTripIds = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BatchViewModel model)
        {

            var batch = _batchService.GetById(model.BatchId);

            var ctx = Users.UserContext();

            batch.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    batch.Accounts = new List<Database.Models.Business.Account>();
                    if (model.AccountIds != null)
                    {
                        var ids = model.AccountIds.Split(',');
                        foreach (var account in ids)
                        {
                            var s = account.Replace("BOTS", "").Replace("SWAZ", "");
                            if (!String.IsNullOrEmpty(s))
                            {
                                var id = Int32.Parse(s);
                                if (id > 0)
                                {
                                    var a = _accountService.GetById(id);
                                    batch.Accounts.Add(a);
                                }
                            }
                        }
                    }

                    batch.BorderTrips = new List<Database.Models.Business.BorderTrip>();
                    if (model.BorderTripIds != null)
                    {
                        var borderTripIds = model.BorderTripIds.Split(',');
                        foreach (var borderTrip in borderTripIds)
                        {
                            var s = borderTrip.Replace("BT", "");
                            if (!String.IsNullOrEmpty(s))
                            {
                                var id = Int32.Parse(s);
                                if (id > 0)
                                {
                                    var b = _borderTripService.GetById(id);
                                    batch.BorderTrips.Add(b);
                                }
                            }
                        }
                    }

                    //Update
                    _batchService.Update(batch);

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

            AddNotification("The batch was successfully updated!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.BatchId = batch.BatchId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}