using Milton.Database.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Finance
{
    public interface IPaymentService
    {
        void Insert(Payment model);
        void Update(Payment model);
        void Delete(Payment model);

        Payment GetById(Int32 id);
        Payment GetByInvoiceNumber(string invoiceNumber);
        List<Payment> GetAll();
    }
}
