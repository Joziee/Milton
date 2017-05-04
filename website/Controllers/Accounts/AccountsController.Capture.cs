using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Accounts
{
    public partial class AccountsController
    {
        public ActionResult Capture()
        {
            ViewBag.Title = "Capture new Patient";
            return View();
        }

        [HttpPost]
        public ActionResult Capture(AccountsViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<AccountsViewModel, Milton.Database.Models.Business.Account>();
            Milton.Database.Models.Business.Account account = AutoMapper.Mapper.Map<AccountsViewModel, Milton.Database.Models.Business.Account>(model);

            var ctx = Users.UserContext();

            account.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _accountsService.Insert(account);

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

            AddNotification("The patient was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.AccountId = account.AccountId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}