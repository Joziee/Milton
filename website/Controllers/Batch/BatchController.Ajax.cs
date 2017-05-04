using Milton.Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Batch
{
    public partial class BatchController
    {
        [HttpGet]
        public JsonResult Json(jQueryDataTableParamModel param)
        {
            Int32 sortingColumn = Int32.Parse(this.Request["iSortCol_0"]);
            String sortDirection = this.Request["sSortDir_0"];

            List<Milton.Database.Models.Business.Batch> batches = _batchService.GetAll().ToList();

            IEnumerable<Milton.Database.Models.Business.Batch> filteredBatches = batches;

            var displayedAccounts = filteredBatches
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);

            var result = displayedAccounts.Select(a => new
            {
                BatchId = a.BatchId,
                Count = a.Accounts.Count,
                SubmissionDate = a.SubmissionDate.ToString("dd MMMM yyyy"),
                BorderTrips = a.BorderTrips.Count
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
                iTotalRecords = batches.Count(),
                iTotalDisplayRecords = filteredBatches.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}