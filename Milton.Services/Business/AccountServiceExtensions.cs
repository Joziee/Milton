using Milton.Database.Models.Business;
using Milton.Services.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Milton.Database.Models.Medical;
using Milton.Database;

namespace Milton.Services.Business
{
    public static class AccountServiceExtensions
    {
        public static bool ChargeDoubleAtDate(this Account account, DateTime dt, List<Admit> admissions, List<Discharge> discharges)
        {
            bool result = false;
            bool gaurdian = account.Gaurdian;

            if (!gaurdian) return result;

            var admitRepo = DependencyResolver.Current.GetService<IDataRepository<Admit>>();
            var dischargeRepo = DependencyResolver.Current.GetService<IDataRepository<Discharge>>();

            var lastAdmission = admissions.Where(x => x.AdmittedDate <= dt).OrderByDescending(x => x.AdmittedDate).FirstOrDefault();
            var lastDischarge = discharges.Where(x => x.DischargeDate <= dt).OrderByDescending(x => x.DischargeDate).FirstOrDefault();

            if (lastDischarge == null && lastAdmission == null) result = true;

            if (lastAdmission != null && lastDischarge != null)
            {
                if ((lastAdmission.AdmittedDate < lastDischarge.DischargeDate) && (lastAdmission.Patient && lastDischarge.Patient)) result = true;
            }

            if (lastAdmission == null && lastDischarge != null)
            {
                if (lastDischarge.Gaurdian && lastDischarge.Patient) result = true;
            }

            return result;
        }
    }
}
