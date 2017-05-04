using Milton.Website.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Accounts
{
    public partial class AccountsController
    {
        /// <summary>
		/// A form to edit a patient
		/// </summary>
		/// <param name="patientId"></param>
		/// <returns></returns>
		[HttpGet]
        public ActionResult Edit(Int32 accountId)
        {
            Milton.Database.Models.Business.Account account = _accountsService.GetById(accountId);

            AccountsViewModel model = new AccountsViewModel()
            {
                ArrivalDate = account.ArrivalDate,
                Gaurdian = account.Gaurdian,
                GaurdianIdNumber = account.GaurdianIdNumber,
                GaurdianName = account.GaurdianName,
                GaurdianSurname = account.GaurdianSurname,
                IdNumber = account.IdNumber,
                Name = account.Name,
                AccountId = account.AccountId,
                RegionId = account.RegionId,
                Surname = account.Surname,
                DateOfBirth = account.DateOfBirth.ToString("MM/dd/yyyy"),
                GaurdianDateOfBirth = account.GaurdianDateOfBirth.HasValue ? account.GaurdianDateOfBirth.Value.ToString("MM/dd/yyyy") : "",
                AuthNumber = account.AuthNumber,
                NeedLog = account.NeedLog,
                HospitalId = account.HospitalId,
                GaurdianBegin = account.GaurdianBegin,
                PatientBegin = account.PatientBegin,
                GaurdianAuthNumber = account.GaurdianAuthNumber
            };

            ViewBag.Title = "Edit Patient (" + model.Name + " " + model.Surname + ")";

            return View(model);
        }

        /// <summary>
		/// Update the patient
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost, ValidateInput(false)]
        public ActionResult Edit(AccountsViewModel model)
        {
            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<AccountsViewModel, Milton.Database.Models.Business.Account>();
            Milton.Database.Models.Business.Account account = AutoMapper.Mapper.Map<AccountsViewModel, Milton.Database.Models.Business.Account>(model);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Update
                    _accountsService.Update(account);

                    //Commit
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("model error", ex);
                    //Error handeled by client
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }

            }

            AddNotification("Successfully updated patient!", true);

            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}