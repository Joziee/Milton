using Milton.Services.Business;
using Milton.Services.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Batch
{
    public partial class BatchController : BaseController
    {
        #region Fields

        protected IBatchService _batchService;
        protected IAccountService _accountService;
        protected IReportService _reportService;
        protected IBorderTripService _borderTripService;

        #endregion

        #region Constructor

        public BatchController(IBatchService batchService, IAccountService accountService, IReportService reportService, IBorderTripService borderTripService)
        {
            _batchService = batchService;
            _accountService = accountService;
            _reportService = reportService;
            _borderTripService = borderTripService;
        }

        #endregion
    }
}