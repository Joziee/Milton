using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Reporting
{
    public interface IReportService
    {
        byte[] GenerateDailyPatients(DateTime date, int regionId, bool console = false);
        byte[] GeneratePatientReport(int accountId);
        byte[] GenerateBatchReport(int batchId);
        byte[] GenerateBorderTripReport(int borderTripId);
        byte[] GenerateLogReport(int regionId, bool console = false);
        List<AccommodationReportItem> CalculateAccommodation(Account account, out decimal patientMealTotal, out decimal patientLodgeTotal, out decimal gaurdianMealTotal, out decimal gaurdianLodgeTotal, out decimal patientLodgeQty, out decimal gaurdianLodgeQty);
    }
}
