using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IReconService
    {
        void Insert(Recon model);
        void Update(Recon model);
        void Delete(Recon model);

        Recon GetById(Int32 id);
        Recon GetByCriteria(DateTime date, string invoiceNumber, decimal amount);
        List<Recon> GetAll();
    }
}
