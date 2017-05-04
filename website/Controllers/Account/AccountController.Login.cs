using Milton.Website.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Account
{
    public partial class AccountController
    {
        [AllowAnonymous]//made a comment
        public ActionResult Login()
        {
            ViewBag.Title = "Login";

            return View();
        }

        [HttpPost, AllowAnonymous, ValidateInput(false)]
        public ActionResult Login(LoginViewModel model)
        {
            Boolean valid = false;
            String sessionId = null;
            var returnmodel = new LoginViewModel();
            returnmodel.Email = model.Email;

            try
            {
                if (ModelState.IsValid)
                {

                    //Attempt to login
                    valid = _securityService.Authenticate(model.Email, model.Password, out sessionId);
                    if (valid && !String.IsNullOrWhiteSpace(sessionId))
                    {
                        DateTime? expires = null;
                        if (model.RememberMe) expires = DateTime.Now.AddDays(60);
                        _securityService.SetAuthCookie(sessionId, expires);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Login", ex.Message);
                return View(returnmodel);
            }

            if (valid)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { SessionId = sessionId, RedirectUrl = "/", Result = true });
                }
                //Check for a redirect url and redirect
                if (!String.IsNullOrWhiteSpace(model.RedirectUrl))
                {
                    //Redirect
                    return Redirect((Request.Url.Scheme + "://" + Request.Url.Authority + model.RedirectUrl).ToLower());
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { Result = false, Message = "Invalid username and password combination! Please try again!" });
                }
                else
                {
                    ModelState.AddModelError("Login", "Invalid username and password combination! Please try again!");
                    return View(returnmodel);
                }
            }

            //By default redirect to home page
            return Redirect("/");
        }
    }
}
