using Milton.Services.Business;
using Milton.Services.Medical;
using Milton.Services.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.BorderTrip
{
    public partial class BorderTripController : BaseController
    {
        #region Fields

        protected IAccountService _accountService;
        protected IBorderTripService _borderTripService;
        protected IReportService _reportService;

        #endregion

        #region Constructor

        public BorderTripController(IAccountService accountService, IBorderTripService borderTripService, IReportService reportService)
        {
            _accountService = accountService;
            _borderTripService = borderTripService;
            _reportService = reportService;
        }

        #endregion
    }
}