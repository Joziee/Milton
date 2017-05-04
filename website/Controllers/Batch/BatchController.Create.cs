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
        public ActionResult Create()
        {
            ViewBag.Title = "Create a Batch";
            return View();
        }

        [HttpPost]
        public ActionResult Create(BatchViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<BatchViewModel, Milton.Database.Models.Business.Batch>();
            Milton.Database.Models.Business.Batch batch = AutoMapper.Mapper.Map<BatchViewModel, Milton.Database.Models.Business.Batch>(model);

            var ctx = Users.UserContext();

            batch.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    batch.Accounts = new List<Database.Models.Business.Account>();

                    if (!String.IsNullOrEmpty(model.AccountIds))
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

                    //Create
                    _batchService.Insert(batch);

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

            AddNotification("The batch was successfully created!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.BatchId = batch.BatchId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}