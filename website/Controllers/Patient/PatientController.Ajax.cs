using Milton.Web.Mvc.Helpers;
using Milton.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController
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

            List<Milton.Database.Models.Business.Account> accounts = _accountService.GetAll(r).ToList();

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
                ArrivalDate = a.ArrivalDate,
                RegionName = a.Region.Name
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
        public ActionResult Verify(string idNumber, string ad, string dd)
        {
            DateTime arrivalDate;
            DateTime departureDate;

            DateTime.TryParse(ad, out arrivalDate);
            DateTime.TryParse(dd, out departureDate);

            var patient = _patientService.GetByCriteria(idNumber);

            if (patient == null) return Json(new { Message = "", Success = true }, JsonRequestBehavior.AllowGet);

            return Json(new { Message = "Patient is already captured!", Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            var patients = _patientService.GetAll();
            string searchTerm = term.ToLower();

            //filter

            var result = patients.Where(x => x.IdNumber.ToLower().Contains(searchTerm.ToLower()));

            return Json(result.Select(x =>
            new
            {
                id = x.PatientId,
                label = x.Name + " " + x.Surname + " (" + x.IdNumber + ") " + (x.Gaurdian ? "with guardian" : ""),
                value = x.IdNumber
            }), JsonRequestBehavior.AllowGet);
        }
    }
}