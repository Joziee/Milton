using Milton.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.BorderTrip
{
    public partial class BorderTripController
    {
        [HttpGet]
        public JsonResult Json(jQueryDataTableParamModel param)
        {
            Int32 sortingColumn = Int32.Parse(this.Request["iSortCol_0"]);
            String sortDirection = this.Request["sSortDir_0"];

            List<Milton.Database.Models.Business.BorderTrip> borderTrips = _borderTripService.GetAll().ToList();

            IEnumerable<Milton.Database.Models.Business.BorderTrip> filteredBorderTrips = borderTrips;

            var displayedBorderTrips = filteredBorderTrips
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);

            var result = displayedBorderTrips.Select(a => new
            {
                BorderTripId = a.BorderTripId,
                Amount = "R" + a.Amount,
                Date = a.Date.ToString("dd MMMM yyyy"),
                Name = a.Name,
                Count = a.Description.Split('\n').Length + 1,
                AccountId = ((a.Account.RegionId == 1) ? "BOTS" : "SWAZ") + a.AccountId.ToString().PadLeft(6, '0')
            }).ToArray();

            //Ordering
            if (sortingColumn > 1)
            {

            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = borderTrips.Count(),
                iTotalDisplayRecords = displayedBorderTrips.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            var borderTrips = _borderTripService.GetAll();
            string searchTerm = term.ToLower();

            //filter

            var result = borderTrips.Where(x => x.BorderTripId.ToString() == searchTerm.ToLower() || x.Name.ToLower().Contains(searchTerm.ToLower()));

            return Json(result.Select(x =>
            new
            {
                id = x.AccountId,
                label = x.Name + " (BT" + x.BorderTripId.ToString().PadLeft(6, '0') + ")",
                bordertripid = "BT" + x.BorderTripId.ToString().PadLeft(6, '0'),
                namesurname = x.Account.Name + " " + x.Account.Surname,
                amount = "R" + x.Amount
            }), JsonRequestBehavior.AllowGet);
        }
    }
}