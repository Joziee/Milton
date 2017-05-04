using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IAccountService
    {
        void Insert(Account model);
        void Update(Account model);
        void Delete(Account model);

        Account GetById(Int32 id);
        List<Account> GetAll(int? regionId = null, bool everyone = true);
        List<Account> GetActive(int? regionId = null);
        Account SearchAccount(string name, string surname);
    }
}
