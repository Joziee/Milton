using Milton.Services.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Patient
{
    public partial class PatientController : BaseController
    {
        #region Fields

        protected IPatientService _patientService;
        protected IAccountService _accountService;

        #endregion

        #region Constructor

        public PatientController(IPatientService patientService, IAccountService accountService)
        {
            _patientService = patientService;
            _accountService = accountService;
        }

        #endregion
    }
}