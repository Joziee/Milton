using Milton.Services.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Discharge
{
    public partial class DischargeController : BaseController
    {
        #region Fields

        protected IDischargeService _dischargeService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public DischargeController(IDischargeService dischargeService, IAccountService accountService)
        {
            _dischargeService = dischargeService;
            _accountService = accountService;
        }

        #endregion
    }
}