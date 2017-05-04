using Milton.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Accounts
{
    public partial class AccountsController
    {
        /// <summary>
		/// Get the data to display in the vendor grid
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
        public JsonResult Json(jQueryDataTableParamModel param)
        {
            Int32 regionId = Int32.Parse(Request["RegionId"]);
            int? r;

            Int32 sortingColumn = Int32.Parse(this.Request["iSortCol_0"]);
            String sortDirection = this.Request["sSortDir_0"];

            if (regionId == 0) r = null; else r = regionId;

            List<Milton.Database.Models.Business.Account> accounts = _accountsService.GetAll(r, false).ToList();

            IEnumerable<Milton.Database.Models.Business.Account> filteredAccounts = accounts;

            //Filtering
            filteredAccounts = !String.IsNullOrWhiteSpace(param.sSearch) ?
                filteredAccounts.Where(m => m.Name.ToLower().Contains(param.sSearch.ToLower()) || m.Surname.ToLower().Contains(param.sSearch.ToLower()) || m.IdNumber.ToLower().Contains(param.sSearch.ToLower()))
                : filteredAccounts;

            var displayedAccounts = filteredAccounts
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);

            var result = displayedAccounts.Select(a => new
            {
                AccountId = a.AccountId,
                Name = a.Name,
                Surname = a.Surname,
                IdNumber = a.IdNumber,
                ArrivalDate = a.ArrivalDate.ToString("MM/dd/yyyy"),
                RegionName = a.Region.Name,
                NeedLog = a.NeedLog
            }).ToArray();

            //Ordering
            if (sortingColumn > 1)
            {
                //switch (sortingColumn)
                //{
                //    case 2:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.Name).ToArray() : result.OrderBy(x => x.Name).ToArray();
                //            break;
                //        }
                //    case 3:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.Manufacturer).ToArray() : result.OrderBy(x => x.Manufacturer).ToArray();
                //            break;
                //        }
                //    case 4:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.Price).ToArray() : result.OrderBy(x => x.Price).ToArray();
                //            break;
                //        }
                //    case 5:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.VariantCount).ToArray() : result.OrderBy(x => x.VariantCount).ToArray();
                //            break;
                //        }
                //    case 6:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.Sku).ToArray() : result.OrderBy(x => x.Sku).ToArray();
                //            break;
                //        }
                //    case 7:
                //        {
                //            result = sortDirection == "desc" ? result.OrderByDescending(x => x.Barcode).ToArray() : result.OrderBy(x => x.Barcode).ToArray();
                //            break;
                //        }
                //}
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = accounts.Count(),
                iTotalDisplayRecords = filteredAccounts.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            bool activeAccounts = true;
            if (Request["activeAccounts"] != null) activeAccounts = false;
            var accounts = _accountsService.GetActive();
            if (!activeAccounts) accounts = _accountsService.GetAll(null, true);
            string searchTerm = term.ToLower();

            //filter

            var result = accounts.Where(x => x.IdNumber.ToLower().Contains(searchTerm.ToLower()) || x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Surname.ToLower().Contains(searchTerm.ToLower()));

            return Json(result.Select(x =>
            new
            {
                id = x.AccountId,
                label = x.Name + " " + x.Surname + " (" + x.IdNumber + ") " + (x.Gaurdian ? "with guardian" : "") + " - Arrival Date " + x.ArrivalDate.ToString("dd/MM/yyyy") + (x.AccountClosed ? " ACCOUNT CLOSED" : ""),
                value = x.IdNumber,
                namesurname = x.Name + " " + x.Surname,
                accountid = ((x.RegionId == 1) ? "BOTS" : "SWAZ") + x.AccountId.ToString().PadLeft(6, '0')
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NeedLog(int accountId, bool needLog)
        {
            var account = _accountsService.GetById(accountId);

            account.NeedLog = needLog;

            _accountsService.Update(account);

            return Json(new { });
        }
    }
}