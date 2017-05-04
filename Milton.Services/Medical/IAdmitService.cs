using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IAdmitService
    {
        void Insert(Admit model);
        void Update(Admit model);
        void Delete(Admit model);

        Admit GetById(Int32 id);
        List<Admit> GetByAccountId(Int32 accountId);
        List<Admit> GetAll();
    }
}
