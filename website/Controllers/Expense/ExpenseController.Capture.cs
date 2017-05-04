using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Expense;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Expense
{
    public partial class ExpenseController
    {
        public ActionResult Capture(int accountId)
        {
            var result = _otherExpenseService.GetByAccountId(accountId);
            var account = _accountService.GetById(accountId);

            ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
            ViewBag.Title = "Capture Other Expense for " + account.Name + " " + account.Surname;
            ViewBag.AccountId = accountId;

            var model = result.Select(x => new ExpenseViewModel()
            {
                Date = x.Date.ToString("dd/MM/yyyy"),
                AccountId = x.AccountId,
                Amount = x.Amount,
                Description = x.Description,
                OtherExpenseId = x.OtherExpenseId,
                SubstractAccommodation = x.SubstractAccommodation
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Capture(ExpenseViewModel model)
        {
            if (model == null) return RedirectToAction("Index");
            int accountId = model.AccountId;

            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<ExpenseViewModel, Milton.Database.Models.Business.OtherExpense>();
            Milton.Database.Models.Business.OtherExpense otherExpense = AutoMapper.Mapper.Map<ExpenseViewModel, Milton.Database.Models.Business.OtherExpense>(model);

            var ctx = Users.UserContext();

            otherExpense.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _otherExpenseService.Insert(otherExpense);

                    //Commit
                    scope.Complete();
                }
                catch (DbEntityValidationException valExc)
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Capture", ex);
                    var result = _otherExpenseService.GetByAccountId(accountId);
                    var account = _accountService.GetById(accountId);

                    ViewBag.Name = account.Name + " " + account.Surname + " (" + account.IdNumber + ")";
                    ViewBag.Title = "Capture Other Expense for " + account.Name + " " + account.Surname;
                    ViewBag.AccountId = accountId;

                    var returnModel = result.Select(x => new ExpenseViewModel()
                    {
                        Date = x.Date.ToString("dd/MM/yyyy"),
                        AccountId = x.AccountId,
                        Amount = x.Amount,
                        Description = x.Description,
                        OtherExpenseId = x.OtherExpenseId
                    }).ToList();

                    return View(returnModel);
                }
            }

            AddNotification("The expense was successfully captured!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.OtherExpenseId = otherExpense.OtherExpenseId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}