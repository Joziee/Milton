using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IPatientService
    {
        void Insert(Patient model);
        void Update(Patient model);
        void Delete(Patient model);

        Patient GetById(Int32 id);
        Patient GetByCriteria(string idNumber);
        List<Patient> GetAll(int? regionId = null);
        List<Patient> Report(DateTime fromDate, DateTime toDate);
        ReportResult TotalInvoices(DateTime startDate, DateTime endDate);
        ReportResult TotalYearToDateInvoices();
        ReportResult TotalMonthToDateCommissionPayable();
        ReportResult TotalYearToDateCommissionPayable();
    }
}
