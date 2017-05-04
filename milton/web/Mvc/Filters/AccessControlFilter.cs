using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Milton.Web.Mvc.Filters
{
    public class AccessControlFilter : System.Web.Mvc.AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// The default login url
        /// </summary>
        private const string DEFAULT_LOGIN_URL = "/account/login";

        /// <summary>
        /// The security service
        /// </summary>
        public Milton.Services.Security.ISecurityService SecurityService { get; set; }

        /// <summary>
        /// The person service
        /// </summary>
        public Milton.Services.Business.IPersonService PersonService { get; set; }

        /// <summary>
        /// The settings service
        /// </summary>

        /// <summary>
        /// The login url to use (instead of the default login url)
        /// </summary>
        public String LoginUrl { get; set; }

        #region Methods
        /// <summary>
        /// Perform a access control check on the currently logged in user
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            //Check if anonymous access is allowed on this action
            if (filterContext.ActionDescriptor.GetCustomAttributes(true).Any(a => a is AllowAnonymousAttribute)) return;

            //Get the request object
            var request = filterContext.HttpContext.Request;

            //Get the details about this request
            var url = request.Url;
            var path = url.PathAndQuery;

            //Generate the login url
            var loginUrl = String.IsNullOrEmpty(this.LoginUrl) ? DEFAULT_LOGIN_URL : this.LoginUrl;

            //Add the redirect to the login url
            loginUrl += "?redirectUrl=" + HttpUtility.UrlEncode(path);

            //Get the person
            String sessionId = null;
            if (!this.SecurityService.CheckAuthCookie(out sessionId)) //User must log in
            {
                filterContext.Result = new RedirectResult(loginUrl);
                return;
            }

            //Get the person
            Guid guid = Guid.Parse(sessionId);
            var person = this.PersonService.GetByGuid(guid);

            //Check for roles
            if (!String.IsNullOrWhiteSpace(this.Roles))
            {
                var roles = this.Roles.Split(',');
                if (!SecurityService.IsInRoles(person.PersonId, roles))
                {
                    filterContext.Result = new RedirectResult(loginUrl); //User doesn't have required role
                    return;
                }

            }
        }

        /// <summary>
        /// Handle the case when a user is not authorized
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
        #endregion
    }
}
