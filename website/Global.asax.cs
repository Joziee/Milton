using Milton.Database.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Milton.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Start the Milton application engine
            var engine = EngineContext.Initialize(false);

            // Installs the plugins on the engine
            engine.InstallAllPlugins();

            //Register the routes in all areas
            AreaRegistration.RegisterAllAreas();

            //Register the default roots
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Register some global filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //Register the bundles
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
