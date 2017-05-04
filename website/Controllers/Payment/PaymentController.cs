using Milton.Services.Finance;
using Milton.Services.Medical;

namespace Milton.Website.Controllers.Payment
{
    public partial class PaymentController : BaseController
    {
        #region Fields

        protected IPaymentService _paymentService;
        protected IHealthShareReconService _healthShareReconService; 

        #endregion

        #region Constructor

        public PaymentController(IPaymentService paymentService, IHealthShareReconService healthShareReconService)
        {
            _paymentService = paymentService;
            _healthShareReconService = healthShareReconService;
        }

        #endregion
    }
}
