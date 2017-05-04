using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IOtherExpenseService
    {
        void Insert(OtherExpense model);
        void Update(OtherExpense model);
        void Delete(OtherExpense model);

        OtherExpense GetById(Int32 id);
        List<OtherExpense> GetByAccountId(Int32 accountId);
        List<OtherExpense> GetAll();
    }
}
