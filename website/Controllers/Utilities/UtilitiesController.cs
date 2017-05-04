using Milton.Services.Finance;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Controllers.Utilities
{
    public partial class UtilitiesController : BaseController
    {
        #region Fields

        protected IPatientService _patientService;
        protected IHealthShareReconService _healthShareReconService;
        protected IReconService _reconService;
        protected IPaymentService _paymentService;

        #endregion

        #region Constructor

        public UtilitiesController(IPatientService patientService, IHealthShareReconService healthShareReconService, IReconService reconService, IPaymentService paymentService)
        {
            _patientService = patientService;
            _healthShareReconService = healthShareReconService;
            _reconService = reconService;
            _paymentService = paymentService;
        }

        #endregion
    }
}
