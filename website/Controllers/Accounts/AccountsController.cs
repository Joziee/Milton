using Milton.Services.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Controllers.Accounts
{
    public partial class AccountsController : BaseController
    {
        #region Fields

        protected IAccountService _accountsService;

        #endregion

        #region Constructor

        public AccountsController(IAccountService accountsController)
        {
            _accountsService = accountsController;
        }

        #endregion
    }
}