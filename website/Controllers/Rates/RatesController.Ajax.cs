using Milton.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Rates
{
    public partial class RatesController
    {
        [HttpGet]
        public JsonResult Json(jQueryDataTableParamModel param)
        {
            Int32 sortingColumn = Int32.Parse(this.Request["iSortCol_0"]);
            String sortDirection = this.Request["sSortDir_0"];

            List<Milton.Database.Models.Medical.Hospital> hospitals = _hospitalService.GetAll();

            IEnumerable<Milton.Database.Models.Medical.Hospital> filteredHospitals = hospitals;

            //Filtering
            filteredHospitals = !String.IsNullOrWhiteSpace(param.sSearch) ?
                filteredHospitals.Where(m => m.Name.ToLower().Contains(param.sSearch.ToLower()))
                : filteredHospitals;

            var displayedHospitals = filteredHospitals
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);

            var result = displayedHospitals.Select(a => new
            {
                HospitalId = a.HospitalId,
                BotswanaPrice = a.BotswanaPrice.HasValue ? ("R" + a.BotswanaPrice.Value) : "R0",
                BotswanaReturnPrice = a.BotswanaReturnPrice.HasValue ? ("R" + a.BotswanaReturnPrice.Value) : "R0",
                Name = a.Name,
                SwazilandPrice = a.SwazilandPrice.HasValue ? ("R" + a.SwazilandPrice.Value) : "R0",
                SwazilandReturnPrice = a.SwazilandReturnPrice.HasValue ? ("R" + a.SwazilandReturnPrice.Value) : "R0"
            }).ToArray();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = hospitals.Count(),
                iTotalDisplayRecords = filteredHospitals.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}