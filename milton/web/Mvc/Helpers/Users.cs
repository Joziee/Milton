using Milton.Database.Models.Business;
using Milton.Services.Business;
using Milton.Services.Security;
using System;
using System.Web.Mvc;

namespace Milton.Web.Mvc.Helpers
{
    public class Users
    {
        /// <summary>
        /// Get the user context
        /// </summary>
        /// <returns></returns>
        public static UserContext UserContext()
        {
            //Resolve dependencies
            IPersonService personService = DependencyResolver.Current.GetService<IPersonService>();
            ISecurityService securityService = DependencyResolver.Current.GetService<ISecurityService>();

            //Create an empty context
            UserContext context = new UserContext();

            //Get the logged on user
            String sessionId = "";
            if (securityService.CheckAuthCookie(out sessionId))
            {
                Guid guid = Guid.Parse(sessionId);
                Person person = personService.GetByGuid(guid);
                context.Person = person;
                context.Person.Roles = person.Roles;
            }

            return context;
        }
    }
}
