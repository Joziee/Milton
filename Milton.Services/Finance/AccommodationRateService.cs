using Milton.Database;
using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milton.Services.Reporting;

namespace Milton.Services.Finance
{
    public class AccommodationRateService : IAccommodationRateService
    {
        #region Fields

        protected IDataRepository<Account> _accountRepo;

        #endregion


        #region Constructor

        public AccommodationRateService(IDataRepository<Account> accountRepo)
        {
            _accountRepo = accountRepo;
        }

        #endregion

        public decimal GetPatientRate(Account account, DateTime dt)
        {
            int age = account.DateOfBirth.GetAge();
            decimal result = 0;

            if (age >= 6 && age <= 10)
            {
                if (account.RegionId == 1)
                {
                    result = 412.16m;
                }
                else
                {
                    result = 351.45m;
                }
            }
            else if (age >= 11)
            {
                if (account.RegionId == 1)
                {
                    result = 824.31m;
                }
                else
                {
                    result = 702.90m;
                }
            }

            return result;
        }
    }
}
