using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Recon
{
    public partial class ReconController
    {
        [HttpGet]
        public ActionResult Refresh()
        {
            //get all the patients
            var patients = _patientService.GetAll().Where(x => x.Done == false);

            //get all the healthsare recons
            var recons = _healthShareReconService.GetAll();

            //assign all the payments
            var unpaidRecons = recons.Where(a => !a.Paid).ToList();
            foreach (var unpaid in unpaidRecons)
            {
                var payment = _paymentService.GetByInvoiceNumber(unpaid.InvoiceNumber);
                if (payment != null)
                {
                    var recon = _healthShareReconService.GetById(unpaid.HealthShareReconId);
                    recon.Paid = true;
                    _healthShareReconService.Update(recon);
                }
            }

            foreach (var patient in patients)
            {
                //get matching recon
                var recon = recons.Where(x => x.IdNumber == patient.IdNumber).ToList();
                if (recon.Count == 0) continue;

                var lodgeTotal = CalculateLodge(patient);
                var transportTotal = CalculateTransport(patient);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }


        private decimal CalculateLodge(Milton.Database.Models.Medical.Patient patient)
        {
            if (patient.IdNumber == "0810032100069")
            {
                var tmp = "";
            }
            decimal total = 0;
            decimal patientRate = 0;
            decimal gaurdianRate = 0;
            decimal patientMealRate = 0;
            decimal gaurdianMealRate = 0;

            if (patient.DateOfBirth == null) return 0;
            if (patient.Gaurdian && patient.GaurdianDateOfBirth == null) return 0;

            int patientAge = patient.DateOfBirth.Value.GetAge();
            bool gaurdian = patient.Gaurdian;
            int? hospitalId = patient.HospitalId;
            int regionId = patient.RegionId;

            if (gaurdian)
            {
                int gaurdianAge = patient.GaurdianDateOfBirth.Value.GetAge();

                if (gaurdianAge > 0 && gaurdianAge <= 5)
                {

                }
                else if (gaurdianAge > 5 && gaurdianAge <= 10)
                {
                    //Botswana
                    if (regionId == 1)
                    {
                        gaurdianRate = 412.16m;
                        gaurdianMealRate = 131.53m;
                    }
                    else
                    {
                        gaurdianRate = 351.45m;
                        gaurdianMealRate = 105.44m;
                    }
                }
                else
                {
                    //Botswana
                    if (regionId == 1)
                    {
                        gaurdianRate = 824.45m;
                        gaurdianMealRate = 263.06m;
                    }
                    else
                    {
                        gaurdianRate = 702.90m;
                        gaurdianMealRate = 210.87m;
                    }
                }
            }

            if (patientAge > 0 && patientAge <= 5)
            {

            }
            else if (patientAge > 5 && patientAge <= 10)
            {
                //Botswana
                if (regionId == 1)
                {
                    patientRate = 412.16m;
                    patientMealRate = 131.53m;
                }
                else
                {
                    patientRate = 351.45m;
                    patientMealRate = 105.44m;
                }
            }
            else
            {
                //Botswana
                if (regionId == 1)
                {
                    patientRate = 824.45m;
                    patientMealRate = 263.06m;
                }
                else
                {
                    patientRate = 702.90m;
                    patientMealRate = 210.87m;
                }
            }

            total += patient.DaysAccommodation * patientRate;
            total += patient.DaysAccommodation * patientMealRate;

            if (gaurdian)
            {
                total += patient.GaurdianDaysAccommodation * gaurdianRate;
                total += patient.DaysAccommodation * gaurdianMealRate;
            }

            //admission and discharged rates
            decimal admittedRate = 0;
            if (regionId == 1) admittedRate = 415.35m;
            else admittedRate = 374.88m;

            decimal dischargedRate = 0;
            if (regionId == 1) dischargedRate = 415.35m;
            else dischargedRate = 374.88m;

            total += patient.AdmittedTransport * admittedRate;
            total += patient.DischargedTransport * dischargedRate;

            return total;
        }

        private decimal CalculateTransport(Milton.Database.Models.Medical.Patient patient)
        {
            int regionId = patient.RegionId;
            decimal total = 0;

            //botswana
            if (regionId == 1)
            {
                total = patient.NormalTransport * 334.41m;
                total += patient.AdmittedTransport * 415.35m;
                total += patient.DischargedTransport * 415.35m;
            }
            else
            {
                total = patient.NormalTransport * 310.98m;
                total += patient.AdmittedTransport * 374.88m;
                total += patient.DischargedTransport * 374.88m;
            }

            return total;
        }

        private int CalculateAge(string idNumber)
        {
            int year = Convert.ToInt32(idNumber.Substring(0, 2));
            int month = Convert.ToInt32(idNumber.Substring(2, 2));
            int day = Convert.ToInt32(idNumber.Substring(4, 2));

            DateTime date = new DateTime(year, month, day);

            return 0;
        }

        private int GetControlDigit(string idNumber)
        {
            int d = -1;
            try
            {
                int a = 0;
                for (int i = 0; i < 6; i++)
                {
                    a += int.Parse(idNumber[2 * i].ToString());
                }
                int b = 0;
                for (int i = 0; i < 6; i++)
                {
                    b = b * 10 + int.Parse(idNumber[2 * i + 1].ToString());
                }
                b *= 2;
                int c = 0;
                do
                {
                    c += b % 10;
                    b = b / 10;
                }
                while (b > 0);
                c += a;
                d = 10 - (c % 10);
                if (d == 10) d = 0;
            }
            catch {/*ignore*/}
            return d;
        }
    }

    public static class AgeExtensions
    {
        public static Int32 GetAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }
    }
}