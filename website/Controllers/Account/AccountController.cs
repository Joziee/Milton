using Milton.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Controllers.Account
{
    public partial class AccountController : BaseController
    {
        #region Fields

        protected ISecurityService _securityService;

        #endregion

        public AccountController(ISecurityService securityService)
        {
            _securityService = securityService;
        }
    }
}
