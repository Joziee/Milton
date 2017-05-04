using Milton.Services.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.AdmitDischarge
{
    public partial class AdmitDischargeController : BaseController
    {
        #region Fields

        protected IAdmitDischargeService _admitDischargeService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public AdmitDischargeController(IAdmitDischargeService admitDischargeService, IAccountService accountService)
        {
            _admitDischargeService = admitDischargeService;
            _accountService = accountService;
        }

        #endregion
    }
}