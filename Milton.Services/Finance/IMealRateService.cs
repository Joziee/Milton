using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Finance
{
    public interface IMealRateService
    {
        decimal GetPatientRate(Account account, DateTime dt);
    }
}
