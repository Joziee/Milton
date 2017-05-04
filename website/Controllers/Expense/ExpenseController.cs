using Milton.Services.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Expense
{
    public partial class ExpenseController : BaseController
    {
        #region Fields

        protected IPatientService _patientService;
        protected IAccountService _accountService;
        protected IOtherExpenseService _otherExpenseService;

        #endregion

        #region Constructor

        public ExpenseController(IPatientService patientService, IAccountService accountService, IOtherExpenseService otherExpenseService)
        {
            _patientService = patientService;
            _accountService = accountService;
            _otherExpenseService = otherExpenseService;
        }

        #endregion
    }
}