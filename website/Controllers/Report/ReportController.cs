using Milton.Services.Medical;
using Milton.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Controllers.Report
{
    [AccessControlFilter(Roles = "Admin")]
    public partial class ReportController : BaseController
    {
        #region Fields

        protected IPatientService _patientService;

        #endregion

        #region Constructor

        public ReportController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        #endregion
    }
}
