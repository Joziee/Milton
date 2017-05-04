using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Account
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Boolean RememberMe { get; set; }
        public string RedirectUrl { get; set; }
    }
}