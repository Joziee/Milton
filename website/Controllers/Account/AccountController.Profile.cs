using Milton.Website.Models.Account;
using Milton.Web.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Account
{
    public partial class AccountController
    {
        public new ActionResult Profile()
        {
            ProfileViewModel model = new ProfileViewModel();

            var person = Users.UserContext().Person;

            model.FirstName = person.FirstName;
            model.Surname = person.Surname;

            ViewBag.Title = "Profile";

            return View(model);
        }
    }
}