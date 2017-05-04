using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IDischargeService
    {
        void Insert(Discharge model);
        void Update(Discharge model);
        void Delete(Discharge model);

        Discharge GetById(Int32 id);
        List<Discharge> GetByAccountId(Int32 accountId);
        List<Discharge> GetAll();
    }
}
