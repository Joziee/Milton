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
        public ActionResult Depart()
        {
            ViewBag.Title = "Patient Depart";
            return View();
        }

        [HttpPost]
        public ActionResult Depart(AccountsViewModel model)
        {
            if (model == null) return RedirectToAction("Index");
            int accountId = model.AccountId;

            var account = _accountsService.GetById(model.AccountId);

            var ctx = Users.UserContext();

            account.ModifiedByUserId = ctx.Person.PersonId;
            account.ModifiedOnUtc = DateTime.Now;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Update
                    account.DepartureDate = model.DepartureDate;
                    account.AccountClosed = true;
                    _accountsService.Update(account);

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

            AddNotification("The account was closed successfully!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.AccountId = account.AccountId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}