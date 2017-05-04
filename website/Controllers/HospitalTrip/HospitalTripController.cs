using Milton.Services.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.HospitalTrip
{
    public partial class HospitalTripController : BaseController
    {
        #region Fields

        protected IHospitalTripService _hospitalTripService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public HospitalTripController(IHospitalTripService hospitalTripService, IAccountService accountService)
        {
            _hospitalTripService = hospitalTripService;
            _accountService = accountService;
        }

        #endregion
    }
}