using Milton.Services.Business;
using Milton.Services.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.GenerateReport
{
    public partial class GenerateReportController : BaseController
    {
        #region Fields

        protected IReportService _reportService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public GenerateReportController(IReportService reportService, IAccountService accountService)
        {
            _reportService = reportService;
            _accountService = accountService;
        }

        #endregion
    }
}