using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Payment
{
    public partial class PaymentController
    {
        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            var healthShareRecons = _healthShareReconService.GetAll();
            string searchTerm = term.ToLower();

            //filter

            var result = healthShareRecons.Where(x => x.InvoiceNumber.ToLower().Contains(searchTerm.ToLower()));

            return Json(result.Select(x =>
            new
            {
                id = x.HealthShareReconId,
                label = x.InvoiceNumber,
                value = x.InvoiceNumber,
                amount = "R" + x.Total.ToString("0.00"),
                name = x.Patient
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Paid(string invoiceNumber, decimal amount, string actionDate)
        {
            var existing = _paymentService.GetByInvoiceNumber(invoiceNumber);

            if (existing == null)
            {
                string date = actionDate.Replace("/", "");
                DateTime dtActionDate = DateTime.ParseExact(date, "MMddyyyy", null);

                Milton.Database.Models.Finance.Payment payment = new Database.Models.Finance.Payment()
                {
                    InvoiceNumber = invoiceNumber,
                    Amount = amount,
                    ActionDate = dtActionDate
                };

                _paymentService.Insert(payment);
            }



            return Json(new { exists = existing != null });
        }
    }
}
