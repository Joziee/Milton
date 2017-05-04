using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Milton.Website.Controllers.Dashboard
{
	public partial class DashboardController
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Dashboard";
			return View();
		}
	}
}
