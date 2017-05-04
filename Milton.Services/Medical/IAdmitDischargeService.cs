using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IAdmitDischargeService
    {
        void Insert(AdmitDischarge model);
        void Update(AdmitDischarge model);
        void Delete(AdmitDischarge model);

        AdmitDischarge GetById(Int32 id);
        List<AdmitDischarge> GetByAccountId(Int32 accountId);
        List<AdmitDischarge> GetAll();
    }
}
