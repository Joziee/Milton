using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Rates
{
    public partial class RatesController : BaseController
    {
        #region Fields

        protected IHospitalService _hospitalService;

        #endregion

        #region Constructor

        public RatesController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        #endregion
    }
}