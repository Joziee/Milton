using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IHealthShareReconService
    {
        void Insert(HealthShareRecon model);
        void Update(HealthShareRecon model);
        void Delete(HealthShareRecon model);

        HealthShareRecon GetById(Int32 id);
        HealthShareRecon GetByCriteria(DateTime invoiceDate, string idNumber, string invoiceNumber, double amount);
        List<HealthShareRecon> GetAll();
    }
}
