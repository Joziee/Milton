using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.BorderTrip
{
    public partial class BorderTripController
    {
        public ActionResult Report(int borderTripId)
        {
            var result = _reportService.GenerateBorderTripReport(borderTripId);

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BT" + borderTripId.ToString().PadLeft(6, '0') + ".xlsx");
        }
    }
}