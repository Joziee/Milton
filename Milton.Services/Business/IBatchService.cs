using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IBatchService
    {
        void Insert(Batch model);
        void Update(Batch model);
        void Delete(Batch model);

        Batch GetById(Int32 id);
        List<Batch> GetByAccountId(Int32 accountId);
        List<Batch> GetAll();
    }
}
