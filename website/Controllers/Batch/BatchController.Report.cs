using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Batch
{
    public partial class BatchController
    {
        public ActionResult Report(int batchId)
        {
            var result = _reportService.GenerateBatchReport(batchId);

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BATCH" + batchId.ToString().PadLeft(6, '0') + ".xlsx");
        }
    }
}