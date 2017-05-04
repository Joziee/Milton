using Milton.Services.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Admit
{
    public partial class AdmitController : BaseController
    {
        #region Fields

        protected IAdmitService _admitService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public AdmitController(IAdmitService admitService, IAccountService accountService)
        {
            _admitService = admitService;
            _accountService = accountService;
        }

        #endregion
    }
}