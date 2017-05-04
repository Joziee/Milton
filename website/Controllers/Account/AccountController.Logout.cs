using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Account
{
    public partial class AccountController
    {
        [HttpGet, Route("account/logout"), AllowAnonymous]
        public ActionResult Logout()
        {
            HttpCookie userAuth = Request.Cookies["UserAuth"];

            if (userAuth != null)
            {
                userAuth.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Set(userAuth);
            }

            return Redirect("/");
        }
    }
}