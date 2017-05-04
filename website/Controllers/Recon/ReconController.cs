using Milton.Services.Finance;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Controllers.Recon
{
    public partial class ReconController : BaseController
    {
        #region Fields

        protected IPatientService _patientService;
        protected IHealthShareReconService _healthShareReconService;
        protected IPaymentService _paymentService;

        #endregion

        #region Constructor

        public ReconController(IPatientService patientService, IHealthShareReconService healthShareReconService, IPaymentService paymentService)
        {
            _patientService = patientService;
            _healthShareReconService = healthShareReconService;
            _paymentService = paymentService;
        }

        #endregion
    }
}
