using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milton.Services.Reporting;

namespace Milton.Services.Finance
{
    public class MealRateService : IMealRateService
    {
        public decimal GetPatientRate(Account account, DateTime dt)
        {
            int age = account.DateOfBirth.GetAge();
            decimal result = 0;

            if (age >= 6 && age <= 10)
            {
                if (account.RegionId == 1)
                {
                    result = 131.53m;
                }
                else
                {
                    result = 105.44m;
                }
            }
            else if (age >= 11)
            {
                if (account.RegionId == 1)
                {
                    result = 263.06m;
                }
                else
                {
                    result = 210.87m;
                }
            }

            return result;
        }
    }
}
