using Milton.Database;
using Milton.Database.Models.Business;
using Milton.Services.Business;
using Milton.Database.Models.Medical;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Milton.Services.Reporting
{
    public class ReportService : IReportService
    {
        #region Fields

        protected IDataRepository<Account> _accountRepo;
        protected IDataRepository<Admit> _admitRepo;
        protected IDataRepository<Discharge> _dischargeRepo;
        protected IDataRepository<HospitalTrip> _hospitalTripRepo;
        protected IDataRepository<Batch> _batchRepo;
        protected IDataRepository<OtherExpense> _otherExpenseRepo;
        protected IDataRepository<DailyReportLog> _dailyReportLogRepo;
        protected IDataRepository<BorderTrip> _borderTripRepo;

        #endregion

        #region Constructor

        public ReportService(IDataRepository<Account> accountRepo, IDataRepository<Admit> admitRepo, IDataRepository<Discharge> dischargeRepo, IDataRepository<HospitalTrip> hospitalTripRepo, IDataRepository<Batch> batchRepo, IDataRepository<OtherExpense> otherExpenseRepo, IDataRepository<DailyReportLog> dailyReportLogRepo, IDataRepository<BorderTrip> borderTripRepo)
        {
            _accountRepo = accountRepo;
            _admitRepo = admitRepo;
            _dischargeRepo = dischargeRepo;
            _hospitalTripRepo = hospitalTripRepo;
            _batchRepo = batchRepo;
            _otherExpenseRepo = otherExpenseRepo;
            _dailyReportLogRepo = dailyReportLogRepo;
            _borderTripRepo = borderTripRepo;
        }

        #endregion

        public byte[] GenerateLogReport(int regionId, bool console = false)
        {
            string part = "BOTS";
            if (regionId == 2)
            {
                part = "SWAZ";
            }
            string path = HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\NeedLogTemplate.xlsx";
            if (console) path = @"C:\ConcordiaTemplates\NeedLogTemplate.xlsx";

            byte[] result = null;
            var accounts = (from account in _accountRepo.Table
                            where account.NeedLog == true
                            where account.RegionId == regionId
                            select account).ToList();

            IWorkbook workbook = null;
            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.Open(path);
            }
            catch (Exception)
            {
                return result;
            }

            try
            {
                int rowCount = 1;
                IWorksheet sheet = workbook.Worksheets[0];
                var cells = sheet.Cells;

                foreach (var account in accounts)
                {
                    cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                    cells[rowCount, 1].Value = String.IsNullOrEmpty(account.AuthNumber) ? "N/A" : account.AuthNumber;
                    cells[rowCount, 2].Value = account.Name;
                    cells[rowCount, 3].Value = account.Surname;
                    cells[rowCount, 4].Value = "'" + account.IdNumber;
                    cells[rowCount, 5].Value = "'" + account.ArrivalDate.ToString("yyyy/MM/dd");
                    cells[rowCount, 6].Value = (account.DepartureDate.Value == DateTime.MinValue ? "N/A" : "'" + account.DepartureDate.Value.ToString("yyyy/MM/dd"));
                    rowCount++;
                }

                MemoryStream ms = new MemoryStream();

                workbook.SaveToStream(ms, FileFormat.OpenXMLWorkbook);
                result = ms.ToArray();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }

        public byte[] GenerateDailyPatients(DateTime date, int regionId, bool console = false)
        {
            List<int> admitIds = new List<int>();
            List<int> dischargeIds = new List<int>();
            List<int> hospitalTripIds = new List<int>();

            DateTime previousDate = date.AddDays(-1);
            string pathPart = "Botswana";
            string part = "BOTS";
            if (regionId == 2)
            {
                pathPart = "Swaziland";
                part = "SWAZ";
            }
            string path = HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\CaseManagement" + pathPart + "Template.xlsx";
            if (console) path = @"C:\ConcordiaTemplates\CaseManagement" + pathPart + "Template.xlsx";

            byte[] result = null;
            var accounts = (from account in _accountRepo.Table.Include(x => x.Hospital)
                            where date >= account.ArrivalDate
                            where account.RegionId == regionId
                            select account).ToList();

            var lastReport = _dailyReportLogRepo.Table.OrderByDescending(x => x.LastSent).FirstOrDefault();
            var lastReportDate = date;
            if (console) lastReportDate = lastReport.LastSent.AddHours(-4);

            IWorkbook workbook = null;
            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.Open(path);
            }
            catch (Exception)
            {
                return result;
            }

            try
            {
                int rowCount = 1;
                int transportRowCount = 1;
                int minifiedRowCount = 1;
                IWorksheet sheet = workbook.Worksheets[0];
                IWorksheet transportSheet = workbook.Worksheets[1];
                IWorksheet minifiedSheet = workbook.Worksheets[2];
                var cells = sheet.Cells;
                var transportCells = transportSheet.Cells;
                var minifiedCells = minifiedSheet.Cells;

                int totalPatientsInLodge = 0;
                int totalGaurdiansInLodge = 0;
                int totalDepartures = 0;
                int totalAdmissions = 0;
                int totalDischarges = 0;

                foreach (var account in accounts)
                {
                    var minifiedItems = new List<MinifiedItem>();

                    var admissions = _admitRepo.Table.Include(x => x.Hospital).Where(x => x.Sent == false && x.AccountId == account.AccountId).ToList();
                    var discharges = _dischargeRepo.Table.Include(x => x.Hospital).Where(x => x.Sent == false && x.AccountId == account.AccountId).ToList();
                    var hospitalTrips = _hospitalTripRepo.Table.Include(x => x.Hospital).Where(x => x.Sent == false && x.AccountId == account.AccountId).ToList();

                    if (!console)
                    {
                        admissions = _admitRepo.Table.Include(x => x.Hospital).Where(x => x.AccountId == account.AccountId && x.AdmittedDate == date).ToList();
                        discharges = _dischargeRepo.Table.Include(x => x.Hospital).Where(x => x.AccountId == account.AccountId && x.DischargeDate == date).ToList();
                        hospitalTrips = _hospitalTripRepo.Table.Include(x => x.Hospital).Where(x => x.AccountId == account.AccountId && x.Date == date).ToList();
                    }

                    admitIds.AddRange(admissions.Select(x => x.AdmitId));
                    dischargeIds.AddRange(discharges.Select(x => x.DischargeId));
                    hospitalTripIds.AddRange(hospitalTrips.Select(x => x.HospitalTripId));

                    if (account.AccountClosed)
                    {
                        if ((new DateTime(account.DepartureDate.Value.Year, account.DepartureDate.Value.Month, account.DepartureDate.Value.Day).AddDays(1) != new DateTime(date.Year, date.Month, date.Day)) && (date > account.DepartureDate.Value)) continue;
                    }

                    if (new DateTime(account.ArrivalDate.Year, account.ArrivalDate.Month, account.ArrivalDate.Day) == new DateTime(date.Year, date.Month, date.Day)) continue;

                    //check for other expense
                    bool hasOtherExpense = _otherExpenseRepo.Table.Where(a => a.AccountId == account.AccountId).Any(a => a.SubstractAccommodation);
                    if (hasOtherExpense) continue;

                    var lastAdmission = _admitRepo.Table.Where(x => x.AccountId == account.AccountId).OrderByDescending(x => x.AdmittedDate).FirstOrDefault();
                    var lastDischarge = _dischargeRepo.Table.Where(x => x.AccountId == account.AccountId).OrderByDescending(x => x.DischargeDate).FirstOrDefault();
                    if (lastAdmission != null && lastDischarge != null)
                    {
                        if (lastDischarge.DischargeDate < lastAdmission.AdmittedDate) lastDischarge = null;
                    }

                    //if (lastAdmission != null)
                    //{
                    //    if (lastAdmission.AdmittedDate == date.AddDays(-1)) lastAdmission = null;
                    //}
                    //if (lastDischarge != null)
                    //{
                    //    if (lastDischarge.DischargeDate == date.AddDays(-1)) lastDischarge = null;
                    //}

                    string doctor = "";
                    string hospitalName = "N/A";
                    if (account.Hospital != null)
                    {
                        hospitalName = account.Hospital.Name;
                    }

                    string admissionDate = "";
                    string dischargeDate = "";
                    string departureDate = "N/A";
                    if (account.AccountClosed)
                    {
                        if (date >= account.DepartureDate.Value)
                        {
                            departureDate = account.DepartureDate.Value.ToString("dd/MM/yyyy");
                            totalDepartures++;
                        }
                    }

                    bool skipPatient = false;
                    if (lastAdmission != null)
                    {
                        if (lastAdmission.Patient)
                        {
                            admissionDate = "Admitted " + lastAdmission.AdmittedDate.ToString("yyyy/MM/dd");
                            doctor = lastAdmission.DoctorName;
                            if (String.IsNullOrEmpty(doctor)) doctor = "N/A";
                            totalAdmissions++;
                        }
                    }

                    if (lastDischarge != null)
                    {
                        if (lastDischarge.Patient)
                        {
                            dischargeDate = "Discharged " + lastDischarge.DischargeDate.ToString("yyyy/MM/dd");
                            doctor = "";
                            totalDischarges++;
                        }
                    }

                    if (account.PatientBegin == false && !skipPatient)
                    {
                        cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0') + ".0";
                        cells[rowCount, 1].Value = String.IsNullOrEmpty(account.AuthNumber) ? "N/A" : account.AuthNumber;
                        cells[rowCount, 2].Value = account.Name;
                        cells[rowCount, 3].Value = account.Surname;
                        cells[rowCount, 4].Value = "'" + account.IdNumber;
                        cells[rowCount, 5].Value = "'" + account.ArrivalDate.ToString("yyyy/MM/dd");
                        cells[rowCount, 6].Value = departureDate;
                        cells[rowCount, 7].Value = hospitalName;
                        cells[rowCount, 8].Value = account.Gaurdian ? "Child" : "Patient";
                        cells[rowCount, 9].Value = account.NeedLog ? "Yes" : "";
                        cells[rowCount, 10].Value = admissionDate;
                        cells[rowCount, 11].Value = dischargeDate;
                        if (String.IsNullOrEmpty(dischargeDate))
                        {
                            if (String.IsNullOrEmpty(admissionDate) && departureDate == "N/A")
                            {
                                cells[rowCount, 12].Value = "At Lodge";
                                totalPatientsInLodge++;
                                minifiedItems.Add(new MinifiedItem()
                                {
                                    AccountId = part + account.AccountId.ToString().PadLeft(6, '0') + ".0",
                                    AdmissionDate = admissionDate,
                                    ArrivalDate = account.ArrivalDate.ToString("yyyy/MM/dd"),
                                    AuthNumber = String.IsNullOrEmpty(account.AuthNumber) ? "N/A" : account.AuthNumber,
                                    DepartureDate = departureDate,
                                    DischargedDate = dischargeDate,
                                    IdNumber = account.IdNumber,
                                    Name = account.Name,
                                    Surname = account.Surname
                                });
                            }
                            else
                            {
                                cells[rowCount, 12].Value = "";
                            }
                        }
                        else
                        {
                            if (departureDate == "N/A")
                            {
                                cells[rowCount, 12].Value = "At Lodge";
                                totalPatientsInLodge++;
                                minifiedItems.Add(new MinifiedItem()
                                {
                                    AccountId = part + account.AccountId.ToString().PadLeft(6, '0') + ".0",
                                    AdmissionDate = admissionDate,
                                    ArrivalDate = account.ArrivalDate.ToString("yyyy/MM/dd"),
                                    AuthNumber = String.IsNullOrEmpty(account.AuthNumber) ? "N/A" : account.AuthNumber,
                                    DepartureDate = departureDate,
                                    DischargedDate = dischargeDate,
                                    IdNumber = account.IdNumber,
                                    Name = account.Name,
                                    Surname = account.Surname
                                });
                            }
                            else
                            {
                                cells[rowCount, 12].Value = "";
                            }
                        }

                        cells[rowCount, 13].Value = doctor;
                    }
                    else
                    {
                        rowCount = rowCount - 1;
                    }

                    //gaurdian starts
                    if (account.Gaurdian)
                    {
                        dischargeDate = "";
                        admissionDate = "";
                        if (lastAdmission != null)
                        {
                            if (lastAdmission.Gaurdian)
                            {
                                admissionDate = "Admitted " + lastAdmission.AdmittedDate.ToString("yyyy/MM/dd");
                                doctor = lastAdmission.DoctorName;
                                if (String.IsNullOrEmpty(doctor)) doctor = "N/A";
                                totalAdmissions++;
                            }
                        }

                        if (lastDischarge != null)
                        {
                            if (lastDischarge.Gaurdian)
                            {
                                dischargeDate = "Discharged " + lastDischarge.DischargeDate.ToString("yyyy/MM/dd");
                                doctor = "";
                                totalDischarges++;
                            }
                        }

                        int newRow = rowCount + 1;
                        cells[newRow, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0') + ".1";
                        cells[newRow, 1].Value = String.IsNullOrEmpty(account.GaurdianAuthNumber) ? "N/A" : account.GaurdianAuthNumber;
                        cells[newRow, 2].Value = account.GaurdianName;
                        cells[newRow, 3].Value = account.GaurdianSurname;
                        cells[newRow, 4].Value = "'" + account.GaurdianIdNumber;
                        cells[newRow, 5].Value = "'" + account.ArrivalDate.ToString("yyyy/MM/dd");
                        cells[newRow, 6].Value = departureDate;
                        cells[newRow, 7].Value = hospitalName;
                        cells[newRow, 8].Value = "Parent";
                        cells[newRow, 9].Value = account.NeedLog ? "Yes" : "";
                        cells[newRow, 10].Value = admissionDate;
                        cells[newRow, 11].Value = dischargeDate;
                        if (String.IsNullOrEmpty(dischargeDate))
                        {
                            if (String.IsNullOrEmpty(admissionDate) && departureDate == "N/A")
                            {
                                cells[newRow, 12].Value = "At Lodge";
                                totalGaurdiansInLodge++;
                                minifiedItems.Add(new MinifiedItem()
                                {
                                    AccountId = part + account.AccountId.ToString().PadLeft(6, '0') + ".1",
                                    AdmissionDate = admissionDate,
                                    ArrivalDate = account.ArrivalDate.ToString("yyyy/MM/dd"),
                                    AuthNumber = String.IsNullOrEmpty(account.GaurdianAuthNumber) ? "N/A" : account.GaurdianAuthNumber,
                                    DepartureDate = departureDate,
                                    DischargedDate = dischargeDate,
                                    IdNumber = account.GaurdianIdNumber,
                                    Name = account.GaurdianName,
                                    Surname = account.GaurdianSurname
                                });
                            }
                            else
                            {
                                cells[newRow, 12].Value = "";
                            }
                        }
                        else
                        {
                            if (departureDate == "N/A")
                            {
                                cells[newRow, 12].Value = "At Lodge";
                                totalGaurdiansInLodge++;
                                minifiedItems.Add(new MinifiedItem()
                                {
                                    AccountId = part + account.AccountId.ToString().PadLeft(6, '0') + ".1",
                                    AdmissionDate = admissionDate,
                                    ArrivalDate = account.ArrivalDate.ToString("yyyy/MM/dd"),
                                    AuthNumber = String.IsNullOrEmpty(account.GaurdianAuthNumber) ? "N/A" : account.GaurdianAuthNumber,
                                    DepartureDate = departureDate,
                                    DischargedDate = dischargeDate,
                                    IdNumber = account.GaurdianIdNumber,
                                    Name = account.GaurdianName,
                                    Surname = account.GaurdianSurname
                                });
                            }
                            else
                            {
                                cells[newRow, 12].Value = "";
                            }
                        }

                        cells[newRow, 13].Value = doctor;

                        if (departureDate != "N/A") totalDepartures++;
                    }

                    if (account.Gaurdian) rowCount += 2;
                    else rowCount++;

                    //transport sheet

                    foreach (var admit in admissions)
                    {
                        transportCells[transportRowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        transportCells[transportRowCount, 1].Value = account.Name + " " + account.Surname;
                        transportCells[transportRowCount, 2].Value = "'" + account.IdNumber;
                        transportCells[transportRowCount, 3].Value = "Admission to " + admit.Hospital.Name;
                        transportCells[transportRowCount, 4].Value = "'" + admit.AdmittedDate.ToString("yyyy/MM/dd");
                        transportRowCount++;
                    }

                    foreach (var discharge in discharges)
                    {
                        transportCells[transportRowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        transportCells[transportRowCount, 1].Value = account.Name + " " + account.Surname;
                        transportCells[transportRowCount, 2].Value = "'" + account.IdNumber;
                        transportCells[transportRowCount, 3].Value = "Discharge from " + discharge.Hospital.Name;
                        transportCells[transportRowCount, 4].Value = "'" + discharge.DischargeDate.ToString("yyyy/MM/dd");
                        transportRowCount++;
                    }

                    foreach (var hospitalTrip in hospitalTrips)
                    {
                        transportCells[transportRowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        if (account.Gaurdian)
                        {
                            transportCells[transportRowCount, 1].Value = account.GaurdianName + " " + account.GaurdianSurname;
                            transportCells[transportRowCount, 2].Value = "'" + account.GaurdianIdNumber;
                        }
                        else
                        {
                            transportCells[transportRowCount, 1].Value = account.Name + " " + account.Surname;
                            transportCells[transportRowCount, 2].Value = "'" + account.IdNumber;
                        }

                        transportCells[transportRowCount, 3].Value = (hospitalTrip.ReturnTrip ? "Return " : "") + "Hospital Trip to/from " + hospitalTrip.Hospital.Name;
                        transportCells[transportRowCount, 4].Value = "'" + hospitalTrip.Date.ToString("yyyy/MM/dd");
                        transportRowCount++;
                    }

                    foreach (var minified in minifiedItems)
                    {
                        minifiedCells[minifiedRowCount, 0].Value = minified.AccountId;
                        minifiedCells[minifiedRowCount, 1].Value = "'" + minified.AuthNumber;
                        minifiedCells[minifiedRowCount, 2].Value = minified.Name;
                        minifiedCells[minifiedRowCount, 3].Value = minified.Surname;
                        minifiedCells[minifiedRowCount, 4].Value = "'" + minified.IdNumber;
                        minifiedCells[minifiedRowCount, 5].Value = "'" + minified.ArrivalDate;
                        minifiedCells[minifiedRowCount, 6].Value = "'" + minified.DepartureDate;
                        minifiedCells[minifiedRowCount, 7].Value = "'" + minified.AdmissionDate;
                        minifiedCells[minifiedRowCount, 8].Value = "'" + minified.DischargedDate;
                        minifiedRowCount++;
                    }
                }

                rowCount++;
                cells[rowCount, 10].Value = "Total Patients Currently at Lodge";
                cells[rowCount, 10].Font.Bold = true;
                cells[rowCount, 11].Value = totalPatientsInLodge;
                rowCount++;
                cells[rowCount, 10].Value = "Total Gaurdians Currently at Lodge";
                cells[rowCount, 10].Font.Bold = true;
                cells[rowCount, 11].Value = totalGaurdiansInLodge;
                rowCount++;
                cells[rowCount, 10].Value = "Total Admissions";
                cells[rowCount, 10].Font.Bold = true;
                cells[rowCount, 11].Value = totalAdmissions;
                rowCount++;
                cells[rowCount, 10].Value = "Total Discharges";
                cells[rowCount, 10].Font.Bold = true;
                cells[rowCount, 11].Value = totalDischarges;
                rowCount++;
                cells[rowCount, 10].Value = "Total Departures";
                cells[rowCount, 10].Font.Bold = true;
                cells[rowCount, 11].Value = totalDepartures;

                //set the sent status
                if (console)
                {
                    foreach (var id in admitIds)
                    {
                        var admit = _admitRepo.Table.FirstOrDefault(x => x.AdmitId == id);
                        if (admit != null)
                        {
                            admit.Sent = true;
                            _admitRepo.Update(admit);
                        }
                    }

                    foreach (var id in dischargeIds)
                    {
                        var discharge = _dischargeRepo.Table.FirstOrDefault(x => x.DischargeId == id);
                        if (discharge != null)
                        {
                            discharge.Sent = true;
                            _dischargeRepo.Update(discharge);
                        }
                    }

                    foreach (var id in hospitalTripIds)
                    {
                        var hospitalTrip = _hospitalTripRepo.Table.FirstOrDefault(x => x.HospitalTripId == id);
                        if (hospitalTrip != null)
                        {
                            hospitalTrip.Sent = true;
                            _hospitalTripRepo.Update(hospitalTrip);
                        }
                    }
                }

                MemoryStream ms = new MemoryStream();

                workbook.SaveToStream(ms, FileFormat.OpenXMLWorkbook);

                result = ms.ToArray();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }

        public byte[] GeneratePatientReport(int accountId)
        {
            string path = HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\PatientReportTemplate.xlsx";
            //string path = @"D:\Projects\Milton\src\Website\App_Data\PatientReportTemplate.xlsx";

            byte[] result = null;
            var account = _accountRepo.Table.FirstOrDefault(x => x.AccountId == accountId);
            var otherExpenses = _otherExpenseRepo.Table.Where(x => x.AccountId == accountId).ToList();

            IWorkbook workbook = null;
            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.Open(path);
            }
            catch (Exception)
            {
                return result;
            }

            try
            {
                //Sheet
                IWorksheet sheet = workbook.Worksheets[0];
                PartialPatientReport(sheet, account, workbook, otherExpenses);

                MemoryStream ms = new MemoryStream();

                workbook.SaveToStream(ms, FileFormat.OpenXMLWorkbook);
                result = ms.ToArray();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }

        private void PartialPatientReport(IWorksheet sheet, Account account, IWorkbook workbook, List<OtherExpense> otherExpenses)
        {
            int rowCount = 4;
            int accountId = account.AccountId;

            sheet.PageSetup.FitToPagesWide = 1;
            sheet.PageSetup.Orientation = PageOrientation.Landscape;

            var cells = sheet.Cells;

            cells[0, 0].Value = "Patient:";
            cells[0, 0].Font.Bold = true;
            cells[0, 0].ColumnWidth = 40;

            cells[0, 3].Value = "Arrival Date:";
            cells[0, 3].Font.Bold = true;
            cells[0, 3].ColumnWidth = 15;

            cells[0, 5].Value = "Patient Medical Number:";
            cells[0, 5].Font.Bold = true;
            cells[0, 5].ColumnWidth = 25;

            cells[0, 1].Value = account.Name + " " + account.Surname;
            cells[0, 1].ColumnWidth = 40;
            cells[0, 4].Value = "'" + account.ArrivalDate.ToString("dd/MM/yyyy");
            cells[0, 4].ColumnWidth = 15;

            string part = "BOTS";
            if (account.RegionId == 2) part = "SWAZ";
            cells[2, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');

            if (account.Gaurdian)
            {
                cells[1, 0].Value = "Guardian:";
                cells[1, 1].Value = account.GaurdianName + " " + account.GaurdianSurname;

                cells[1, 5].Value = "Parent Passport Number";
                cells[1, 5].Font.Bold = true;
                cells[1, 6].Value = "'" + account.GaurdianIdNumber;
            }

            cells[1, 4].Value = String.IsNullOrEmpty(account.AuthNumber) ? "N/A" : account.AuthNumber;
            cells[0, 6].Value = "'" + account.IdNumber;
            cells[0, 6].ColumnWidth = 15;

            decimal totalTransport = 0;
            int tap = 0;
            int tag = 0;
            int tdp = 0;
            int tdg = 0;
            int thp = 0;
            int thg = 0;
            var reportResult = GetPatientReportLines(account, out totalTransport, out tap, out tag, out tdp, out tdg, out thp, out thg);

            //calculate accommodation
            decimal totalPatientMeal = 0;
            decimal totalPatientLodge = 0;
            decimal totalGaurdianMeal = 0;
            decimal totalGaurdianLodge = 0;
            decimal totalPatientLodgeQty = 0;
            decimal totalGaurdianLodgeQty = 0;

            decimal totalOtherExpenses = 0;

            var accommodation = CalculateAccommodation(account, out totalPatientMeal, out totalPatientLodge, out totalGaurdianMeal, out totalGaurdianLodge, out totalPatientLodgeQty, out totalGaurdianLodgeQty);

            if (otherExpenses.Any(x => x.SubstractAccommodation == true))
            {
                totalGaurdianLodge = 0;
                totalGaurdianLodgeQty = 0;
                totalGaurdianMeal = 0;
                totalPatientLodge = 0;
                totalPatientLodgeQty = 0;
                totalPatientMeal = 0;
            }
            else
            {
                foreach (var a in accommodation)
                {
                    reportResult.Add(new PatientReportItem()
                    {
                        Cost = a.Cost,
                        Date = a.Date,
                        Description = a.Description,
                        ExtraInfo = ""
                    });
                }
            }

            if (otherExpenses.Any())
            {
                var oeList = otherExpenses.ToList();
                foreach (var oe in oeList)
                {
                    totalOtherExpenses += oe.Amount;

                    reportResult.Add(new PatientReportItem()
                    {
                        Cost = oe.Amount,
                        Date = oe.Date,
                        Description = oe.Description,
                        ExtraInfo = ""
                    });
                }
            }

            reportResult = reportResult.OrderBy(a => a.Date).ToList();

            foreach (var item in reportResult)
            {
                cells[rowCount, 0].Value = item.Description;
                cells[rowCount, 1].Value = "'" + item.Date.ToString("dd/MM/yyyy");
                cells[rowCount, 2].Value = item.ExtraInfo;
                cells[rowCount, 4].Value = item.Cost;
                cells[rowCount, 4].NumberFormat = "R#,###.00";

                rowCount++;
            }

            rowCount++;
            cells[rowCount, 0].Value = "Departure";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = account.AccountClosed ? "'" + account.DepartureDate.Value.ToString("dd/MM/yyyy") : "N/A";

            cells[rowCount, 2].Value = "Total Transport";
            cells[rowCount, 2].Font.Bold = true;
            cells[rowCount, 2].ColumnWidth = 20;
            cells[rowCount, 3].Value = totalTransport;
            cells[rowCount, 3].NumberFormat = "R#,###.00";

            rowCount++;
            cells[rowCount, 0].Value = "Total Hospital Trips - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = thp;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Hospital Trips - Guardian";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = thg;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            cells[rowCount, 2].Value = "Total Accommodation";
            cells[rowCount, 2].Font.Bold = true;
            cells[rowCount, 3].Value = totalGaurdianLodge + totalGaurdianMeal + totalPatientLodge + totalPatientMeal;
            cells[rowCount, 3].NumberFormat = "R#,###.00";

            rowCount++;
            cells[rowCount, 0].Value = "Total Admissions - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = tap;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Admissions - Guardian";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = tag;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            cells[rowCount, 2].Value = "Total Other Expenses";
            cells[rowCount, 2].Font.Bold = true;
            cells[rowCount, 3].Value = totalOtherExpenses;
            cells[rowCount, 3].NumberFormat = "R#,###.00";

            rowCount++;
            cells[rowCount, 0].Value = "Total Discharges - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = tdp;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Discharges - Guardian";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = tdg;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;



            rowCount++;
            cells[rowCount, 0].Value = "Total Accommodation Days - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = totalPatientLodgeQty;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Meals - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = totalPatientLodgeQty;
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Accommodation Amount - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = totalPatientLodge;
            cells[rowCount, 1].NumberFormat = "R#,###.00";
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            rowCount++;
            cells[rowCount, 0].Value = "Total Meals Amount - Patient";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = totalPatientMeal;
            cells[rowCount, 1].NumberFormat = "R#,###.00";
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

            if (account.Gaurdian)
            {
                rowCount++;
                cells[rowCount, 0].Value = "Total Accommodation Days - Parent";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = totalGaurdianLodgeQty;
                cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
                cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

                rowCount++;
                cells[rowCount, 0].Value = "Total Meals - Parent";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = totalGaurdianLodgeQty;
                cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
                cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

                rowCount++;
                cells[rowCount, 0].Value = "Total Accommodation Amount - Parent";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = totalGaurdianLodge;
                cells[rowCount, 1].NumberFormat = "R#,###.00";
                cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
                cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;

                rowCount++;
                cells[rowCount, 0].Value = "Total Meals Amount - Parent";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = totalGaurdianMeal;
                cells[rowCount, 1].NumberFormat = "R#,###.00";
                cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
                cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;
            }

            rowCount++;
            cells[rowCount, 0].Value = "Total Transport";
            cells[rowCount, 0].Font.Bold = true;
            cells[rowCount, 1].Value = totalTransport;
            cells[rowCount, 1].NumberFormat = "R#,###.00";
            cells[rowCount, 1].HorizontalAlignment = HAlign.Left;
            cells[rowCount, 1].Borders[BordersIndex.EdgeBottom].LineStyle = LineStyle.Double;
        }

        public byte[] GenerateBatchReport(int batchId)
        {
            string path = HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\BatchReportTemplate.xlsx";

            byte[] result = null;
            var accountIds = _batchRepo.Table.Include(a => a.Accounts).Where(a => a.BatchId == batchId).SelectMany(a => a.Accounts).Select(a => a.AccountId);
            var accounts = _accountRepo.Table.Include(x => x.Hospital).Where(x => accountIds.Contains(x.AccountId)).ToList();
            var batchBorderTrips = _batchRepo.Table.Include(x => x.BorderTrips).Include(x => x.Accounts).FirstOrDefault(x => x.BatchId == batchId).BorderTrips;
            var borderTripAccountIds = batchBorderTrips.Select(x => x.AccountId).ToList();

            foreach (var borderTripAccountId in borderTripAccountIds)
            {
                accounts.Add(_accountRepo.Table.FirstOrDefault(x => x.AccountId == borderTripAccountId));
            }


            IWorkbook workbook = null;
            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.Open(path);
            }
            catch (Exception)
            {
                return result;
            }

            try
            {
                //Sheet
                int rowCount = 3;
                IWorksheet sheet = workbook.Worksheets[0];
                var cells = sheet.Cells;

                cells[0, 1].Value = "BATCH" + batchId.ToString().PadLeft(6, '0');
                cells[1, 0].Value = "Batch Report - " + accounts.Count + " accounts";

                decimal finalAccommodation = 0;
                decimal finalMeals = 0;
                decimal finalTransport = 0;
                decimal finalOtherExpense = 0;
                decimal finalBorderTrips = 0;
                int count = 1;

                foreach (var account in accounts)
                {
                    var otherExpenses = _otherExpenseRepo.Table.Where(x => x.AccountId == account.AccountId).ToList();
                    string part = "BOTS";
                    if (account.RegionId == 2) part = "SWAZ";

                    IWorksheet accountSheet = workbook.Worksheets.Add();
                    accountSheet.Name = part + account.AccountId.ToString().PadLeft(6, '0');
                    PartialPatientReport(accountSheet, account, workbook, otherExpenses);

                    decimal totalTransport = 0;
                    int tap = 0;
                    int tag = 0;
                    int tdp = 0;
                    int tdg = 0;
                    int thp = 0;
                    int thg = 0;
                    var reportResult = GetPatientReportLines(account, out totalTransport, out tap, out tag, out tdp, out tdg, out thp, out thg);

                    cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                    cells[rowCount, 1].Value = account.Name + " " + account.Surname;
                    cells[rowCount, 2].Value = "'" + account.IdNumber;
                    cells[rowCount, 3].Value = "Concordia Transport";
                    cells[rowCount, 5].Value = totalTransport;
                    cells[rowCount, 5].NumberFormat = "R#,###.00";

                    rowCount++;

                    //calculate accommodation
                    decimal totalPatientMeal = 0;
                    decimal totalPatientLodge = 0;
                    decimal totalGaurdianMeal = 0;
                    decimal totalGaurdianLodge = 0;
                    decimal totalPatientLodgeQty = 0;
                    decimal totalGaurdianLodgeQty = 0;

                    var accommodation = CalculateAccommodation(account, out totalPatientMeal, out totalPatientLodge, out totalGaurdianMeal, out totalGaurdianLodge, out totalPatientLodgeQty, out totalGaurdianLodgeQty);


                    cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                    cells[rowCount, 1].Value = account.Name + " " + account.Surname;
                    cells[rowCount, 2].Value = "'" + account.IdNumber;
                    cells[rowCount, 3].Value = "Concordia Lodge Accommodation - Patient";
                    cells[rowCount, 5].Value = totalPatientLodge + totalPatientMeal;
                    cells[rowCount, 5].NumberFormat = "R#,###.00";

                    rowCount++;

                    if (account.Gaurdian)
                    {
                        cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        cells[rowCount, 1].Value = account.GaurdianName + " " + account.GaurdianSurname;
                        cells[rowCount, 2].Value = "'" + account.GaurdianIdNumber;
                        cells[rowCount, 3].Value = "Concordia Lodge Accommodation - Parent";
                        cells[rowCount, 5].Value = totalGaurdianLodge + totalGaurdianMeal;
                        cells[rowCount, 5].NumberFormat = "R#,###.00";

                        rowCount++;
                    }

                    finalAccommodation += totalGaurdianLodge + totalPatientLodge;
                    finalMeals += totalGaurdianMeal + totalPatientMeal;
                    finalTransport += totalTransport;

                    foreach (var otherExpense in otherExpenses)
                    {
                        cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        cells[rowCount, 1].Value = account.Name + " " + account.Surname;
                        cells[rowCount, 2].Value = "'" + account.IdNumber;
                        cells[rowCount, 3].Value = otherExpense.Description;
                        cells[rowCount, 5].Value = otherExpense.Amount;
                        cells[rowCount, 5].NumberFormat = "R#,###.00";

                        rowCount++;

                        finalOtherExpense += otherExpense.Amount;
                    }

                    var borderTrips = batchBorderTrips.Where(x => x.AccountId == account.AccountId).ToList();

                    foreach (var borderTrip in borderTrips)
                    {
                        cells[rowCount, 0].Value = part + account.AccountId.ToString().PadLeft(6, '0');
                        cells[rowCount, 1].Value = account.Name + " " + account.Surname;
                        cells[rowCount, 2].Value = "'" + account.IdNumber;
                        cells[rowCount, 3].Value = borderTrip.Name;
                        cells[rowCount, 5].Value = borderTrip.Amount;
                        cells[rowCount, 5].NumberFormat = "R#,###.00";

                        rowCount++;

                        finalBorderTrips += borderTrip.Amount;
                    }
                }

                rowCount++;
                cells[rowCount, 0].Value = "Total Concordia Lodge Accommodation";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = finalAccommodation + finalMeals;
                cells[rowCount, 1].NumberFormat = "R#,###.00";

                rowCount++;
                cells[rowCount, 0].Value = "Total Concordia Transport";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = finalTransport;
                cells[rowCount, 1].NumberFormat = "R#,###.00";

                rowCount++;
                cells[rowCount, 0].Value = "Total Other Expense";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = finalOtherExpense;
                cells[rowCount, 1].NumberFormat = "R#,###.00";

                rowCount++;
                cells[rowCount, 0].Value = "Total Border/Bus Trips";
                cells[rowCount, 0].Font.Bold = true;
                cells[rowCount, 1].Value = finalBorderTrips;
                cells[rowCount, 1].NumberFormat = "R#,###.00";

                MemoryStream ms = new MemoryStream();
                workbook.ProtectStructure = false;

                workbook.SaveToStream(ms, FileFormat.OpenXMLWorkbook);

                result = ms.ToArray();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }

        public byte[] GenerateBorderTripReport(int borderTripId)
        {
            string path = HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\BorderTripReportTemplate.xlsx";

            byte[] result = null;
            var borderTrip = _borderTripRepo.Table.Include(x => x.Account).FirstOrDefault(x => x.BorderTripId == borderTripId);
            List<string> patients = borderTrip.Description.Split('\n').ToList();
            int patientCount = patients.Count + 1;

            IWorkbook workbook = null;
            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.Open(path);
            }
            catch (Exception)
            {
                return result;
            }

            try
            {
                //Sheet
                int rowCount = 4;
                IWorksheet sheet = workbook.Worksheets[0];
                var cells = sheet.Cells;

                string part = "SWAZ";
                if (borderTrip.Account.RegionId == 1) part = "BOTS";

                cells[0, 1].Value = "BT" + borderTripId.ToString().PadLeft(6, '0');
                cells[0, 3].Value = part + borderTrip.Account.AccountId.ToString().PadLeft(6, '0');
                cells[0, 5].Value = borderTrip.Account.Name + " " + borderTrip.Account.Surname;
                cells[1, 1].Value = "R" + borderTrip.Amount;
                cells[1, 3].Value = "'" + borderTrip.Date.ToString("dd MMMM yyyy");
                cells[2, 0].Value = "Border/Bus Trip Report - " + patientCount + " patients";

                foreach (var patient in patients)
                {
                    cells[rowCount, 0].Value = patient;
                    rowCount++;
                }

                MemoryStream ms = new MemoryStream();

                workbook.SaveToStream(ms, FileFormat.OpenXMLWorkbook);
                result = ms.ToArray();
                ms.Dispose();
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }


        public List<AccommodationReportItem> CalculateAccommodation(Account account, out decimal patientMealTotal, out decimal patientLodgeTotal, out decimal gaurdianMealTotal, out decimal gaurdianLodgeTotal, out decimal patientLodgeQty, out decimal gaurdianLodgeQty)
        {
            var result = new List<AccommodationReportItem>();

            var admissions = _admitRepo.Table.Include(x => x.Hospital).Where(x => x.AccountId == account.AccountId).ToList();
            var discharges = _dischargeRepo.Table.Include(a => a.Hospital).Where(x => x.AccountId == account.AccountId).ToList();
            var hospitalTrips = _hospitalTripRepo.Table.Include(x => x.Hospital).Where(x => x.AccountId == account.AccountId).ToList();

            var dt = account.DepartureDate.Value;
            if (dt == DateTime.MinValue) dt = DateTime.Now;

            var days = (int)(dt - account.ArrivalDate).TotalDays;

            //combine admissions and discharges
            var admitPatientList = new List<AdmissionDischargeReportItem>();
            var admitGaurdianList = new List<AdmissionDischargeReportItem>();
            var dischargePatientList = new List<AdmissionDischargeReportItem>();
            var dischargeGaurdianList = new List<AdmissionDischargeReportItem>();
            admitPatientList.AddRange(admissions.Where(x => x.Patient).Select(x => new AdmissionDischargeReportItem()
            {
                Date = x.AdmittedDate,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient,
                IsAdmission = true
            }));

            admitGaurdianList.AddRange(admissions.Where(x => x.Gaurdian).Select(x => new AdmissionDischargeReportItem()
            {
                Date = x.AdmittedDate,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient,
                IsAdmission = true
            }));

            dischargePatientList.AddRange(discharges.Where(x => x.Patient).Select(x => new AdmissionDischargeReportItem()
            {
                Date = x.DischargeDate,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient,
                IsAdmission = false
            }));

            dischargeGaurdianList.AddRange(discharges.Where(x => x.Gaurdian).Select(x => new AdmissionDischargeReportItem()
            {
                Date = x.DischargeDate,
                Gaurdian = x.Gaurdian,
                Patient = x.Patient,
                IsAdmission = false
            }));

            admitPatientList = admitPatientList.OrderBy(x => x.Date).ToList();
            admitGaurdianList = admitGaurdianList.OrderBy(x => x.Date).ToList();
            dischargePatientList = dischargePatientList.OrderBy(x => x.Date).ToList();
            dischargeGaurdianList = dischargeGaurdianList.OrderBy(x => x.Date).ToList();

            int totalPatientDays = 0;
            int totalGaurdianDays = 0;
            decimal patientSleepRate = 0;
            decimal patientMealRate = 0;
            decimal gaurdianSleepRate = 0;
            decimal gaurdianMealRate = 0;

            //calculate patient rates
            int patientAge = account.DateOfBirth.GetAge();


            //calculate gaurdian rate


            var admitResult = new List<AdmissionReportItem>();

            // patient admissions
            if (admitPatientList.Count == 0)
            {
                if (discharges.Any())
                {
                    if (discharges.OrderBy(x => x.DischargeDate).FirstOrDefault().DischargeDate == account.ArrivalDate)
                    {
                        if (discharges.OrderBy(x => x.DischargeDate).FirstOrDefault().Patient)
                        {
                            var a = account.ArrivalDate;
                            var t = account.DepartureDate.Value;
                            if (t == DateTime.MinValue) t = DateTime.Now.AddDays(-1);
                            while (a < t)
                            {
                                admitResult.Add(new AdmissionReportItem()
                                {
                                    Date = a,
                                    Gaurdian = false,
                                    Patient = true
                                });

                                a = a.AddDays(1);
                            }
                        }

                    }
                }
                else
                {
                    if (account.PatientBegin == false)
                    {
                        var a = account.ArrivalDate;
                        var t = account.DepartureDate.Value;
                        if (t == DateTime.MinValue) t = DateTime.Now.AddDays(-1);
                        while (a < t)
                        {
                            admitResult.Add(new AdmissionReportItem()
                            {
                                Date = a,
                                Gaurdian = false,
                                Patient = true
                            });

                            a = a.AddDays(1);
                        }
                    }
                }
            }

            for (int i = 0; i < admitPatientList.Count; i++)
            {
                var item = admitPatientList[i];
                var d = 0;


                var previousDischarge = dischargePatientList.OrderByDescending(x => x.Date).FirstOrDefault(x => x.Date < item.Date);
                if (previousDischarge == null)
                {
                    if (account.PatientBegin) continue;

                    var checkDate = account.ArrivalDate;
                    d = (int)(item.Date - checkDate).TotalDays;

                    var tDate = account.ArrivalDate;
                    while (tDate < item.Date)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = tDate,
                            Gaurdian = false,
                            Patient = true
                        });

                        tDate = tDate.AddDays(1);
                    }
                }
                else
                {
                    var checkDate = previousDischarge.Date;
                    d = (int)(item.Date - checkDate).TotalDays;
                    var tDate = item.Date;
                    while (checkDate < tDate)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = checkDate,
                            Gaurdian = false,
                            Patient = true
                        });

                        checkDate = checkDate.AddDays(1);
                    }
                }


                //totalPatientDays += d;
            }

            //gaurdian admissions
            if (admitGaurdianList.Count == 0 && account.Gaurdian)
            {
                if (discharges.Any())
                {
                    if (discharges.OrderBy(x => x.DischargeDate).FirstOrDefault().DischargeDate == account.ArrivalDate)
                    {
                        if (discharges.OrderBy(x => x.DischargeDate).FirstOrDefault().Gaurdian)
                        {
                            var a = account.ArrivalDate;
                            var t = account.DepartureDate.Value;
                            if (t == DateTime.MinValue) t = DateTime.Now.AddDays(-1);
                            while (a < t)
                            {
                                admitResult.Add(new AdmissionReportItem()
                                {
                                    Date = a,
                                    Gaurdian = true,
                                    Patient = false
                                });

                                a = a.AddDays(1);
                            }
                        }
                    }
                    else
                    {
                        var a = account.ArrivalDate;
                        var t = account.DepartureDate.Value;
                        if (t == DateTime.MinValue) t = DateTime.Now.AddDays(-1);
                        while (a < t)
                        {
                            admitResult.Add(new AdmissionReportItem()
                            {
                                Date = a,
                                Gaurdian = true,
                                Patient = false
                            });

                            a = a.AddDays(1);
                        }
                    }
                }
                else
                {
                    if (account.GaurdianBegin == false)
                    {
                        var a = account.ArrivalDate;
                        var tDate = account.DepartureDate.Value;
                        if (tDate == DateTime.MinValue) tDate = DateTime.Now.AddDays(-1);
                        while (a < tDate)
                        {
                            admitResult.Add(new AdmissionReportItem()
                            {
                                Date = a,
                                Gaurdian = true,
                                Patient = false
                            });

                            a = a.AddDays(1);
                        }
                    }
                }
            }

            for (int i = 0; i < admitGaurdianList.Count; i++)
            {
                var item = admitGaurdianList[i];
                var d = 0;

                var previousDischarge = dischargeGaurdianList.OrderByDescending(x => x.Date).FirstOrDefault(x => x.Date < item.Date);
                if (previousDischarge == null)
                {
                    if (account.GaurdianBegin) continue;

                    var checkDate = account.ArrivalDate;
                    d = (int)(item.Date - checkDate).TotalDays;

                    var tDate = account.ArrivalDate;
                    while (tDate < item.Date)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = tDate,
                            Gaurdian = true,
                            Patient = false
                        });

                        tDate = tDate.AddDays(1);
                    }
                }
                else
                {
                    var checkDate = previousDischarge.Date;
                    d = (int)(item.Date - checkDate).TotalDays;
                    var tDate = item.Date;
                    while (checkDate < tDate)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = checkDate,
                            Gaurdian = true,
                            Patient = false
                        });

                        checkDate = checkDate.AddDays(1);
                    }
                }


                //totalGaurdianDays += d;
            }

            var lastPatientAdmit = admitPatientList.LastOrDefault();
            var lastGaurdianAdmit = admitGaurdianList.LastOrDefault();
            var lastPatientDischarge = dischargePatientList.LastOrDefault();
            var lastGaurdianDischarge = dischargeGaurdianList.LastOrDefault();

            if (account.PatientBegin && lastPatientAdmit == null)
            {
                lastPatientAdmit = new AdmissionDischargeReportItem()
                {
                    Date = account.ArrivalDate,
                    Gaurdian = false,
                    IsAdmission = false,
                    Patient = true
                };
            }

            //patient
            if (lastPatientAdmit != null && lastPatientDischarge != null)
            {
                if (lastPatientDischarge.Date > lastPatientAdmit.Date)
                {
                    var tDate = lastPatientDischarge.Date;
                    var eDate = account.DepartureDate.Value;
                    if (eDate == DateTime.MinValue) eDate = DateTime.Now.AddDays(-1);

                    while (tDate < eDate)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = tDate,
                            Gaurdian = false,
                            Patient = true
                        });

                        tDate = tDate.AddDays(1);
                    }
                }
            }

            //gaurdian
            if (lastGaurdianAdmit != null && lastGaurdianDischarge != null)
            {
                if (lastGaurdianDischarge.Date > lastGaurdianAdmit.Date)
                {
                    var tDate = lastGaurdianDischarge.Date;
                    var eDate = account.DepartureDate.Value;
                    if (eDate == DateTime.MinValue) eDate = DateTime.Now.AddDays(-1);

                    while (tDate < eDate)
                    {
                        admitResult.Add(new AdmissionReportItem()
                        {
                            Date = tDate,
                            Gaurdian = true,
                            Patient = false
                        });

                        tDate = tDate.AddDays(1);
                    }
                }
            }

            decimal finalPatientSleepCost = 0;
            decimal finalGaurdianSleepCost = 0;
            decimal finalPatientMealCost = 0;
            decimal finalGaurdianMealCost = 0;

            foreach (var r in admitResult)
            {
                patientSleepRate = 0;
                patientMealRate = 0;
                gaurdianMealRate = 0;
                gaurdianSleepRate = 0;

                if (r.Gaurdian) totalGaurdianDays++;
                else totalPatientDays++;
                if (!r.Gaurdian)
                {
                    patientSleepRate = GetPatientSleepRate(account.RegionId, patientAge, r.Date);
                    patientMealRate = GetPatientMealRate(account.RegionId, patientAge, r.Date);
                }

                if (r.Gaurdian)
                {
                    gaurdianMealRate = 0;
                    gaurdianSleepRate = 0;
                    int gaurdianAge = account.GaurdianDateOfBirth.Value.GetAge();
                    gaurdianMealRate = GetGuardianMealRate(account.RegionId, gaurdianAge, r.Date);
                    gaurdianSleepRate = GetGuardianSleepRate(account.RegionId, gaurdianAge, r.Date);
                }

                result.Add(new AccommodationReportItem()
                {
                    Date = r.Date,
                    Cost = (r.Gaurdian ? (gaurdianSleepRate + gaurdianMealRate) : (patientSleepRate + patientMealRate)),
                    Description = (r.Gaurdian ? "Accommodation + Meals Parent" : "Accommodation + Meals Patient")
                });

                finalPatientSleepCost += patientSleepRate;
                finalPatientMealCost += patientMealRate;
                finalGaurdianSleepCost += gaurdianSleepRate;
                finalGaurdianMealCost += gaurdianMealRate;
            }

            gaurdianLodgeQty = totalGaurdianDays;
            gaurdianMealTotal = finalGaurdianMealCost;
            gaurdianLodgeTotal = finalGaurdianSleepCost;

            patientLodgeQty = totalPatientDays;
            patientLodgeTotal = finalPatientSleepCost;
            patientMealTotal = finalPatientMealCost;

            return result;
        }

        private decimal GetGuardianSleepRate(int regionId, int age, DateTime dt)
        {
            decimal val = 0;

            if (age >= 6 && age <= 10)
            {
                if (regionId == 1)
                {
                    val = 412.16m;
                }
                else
                {
                    val = 351.45m;
                }
            }
            else if (age >= 11)
            {
                if (regionId == 1)
                {
                    val = 824.31m;
                }
                else
                {
                    val = 702.90m;
                }
            }

            if (dt >= new DateTime(2017, 02, 01)) 
                    {
                var tmp = (val * 1.03m);
                return Math.Round(tmp, 2);
            }
            return val;
        }

        private decimal GetPatientSleepRate(int regionId, int age, DateTime dt)
        {
            decimal val = 0;

            if (age >= 6 && age <= 10)
            {
                if (regionId == 1)
                {
                    val = 412.16m;
                }
                else
                {
                    val = 351.45m;
                }
            }
            else if (age >= 11)
            {
                if (regionId == 1)
                {
                    val = 824.31m;
                }
                else
                {
                    val = 702.90m;
                }
            }

            if (dt >= new DateTime(2017, 02, 01))
            {
                var tmp = (val * 1.03m);
                return Math.Round(tmp, 2);
            }
            return val;
        }

        private decimal GetGuardianMealRate(int regionId, int age, DateTime dt)
        {
            decimal val = 0;

            if (age >= 6 && age <= 10)
            {
                if (regionId == 1)
                {
                    val = 131.53m;
                }
                else
                {
                    val = 105.44m;
                }
            }
            else if (age >= 11)
            {
                if (regionId == 1)
                {
                    val = 263.06m;
                }
                else
                {
                    val = 210.87m;
                }
            }

            if (dt >= new DateTime(2017, 02, 01))
            {
                var tmp = (val * 1.03m);
                return Math.Round(tmp, 2);
            }
            return val;
        }

        private decimal GetPatientMealRate(int regionId, int age, DateTime dt)
        {
            decimal val = 0;

            if (age >= 6 && age <= 10)
            {
                if (regionId == 1)
                {
                    val = 131.53m;
                }
                else
                {
                    val = 105.44m;
                }
            }
            else if (age >= 11)
            {
                if (regionId == 1)
                {
                    val = 263.06m;
                }
                else
                {
                    val = 210.87m;
                }
            }

            if (dt >= new DateTime(2017, 02, 01))
            {
                var tmp = (val * 1.03m);
                return Math.Round(tmp, 2);
            }
            return val;
        }

        private List<PatientReportItem> GetPatientReportLines(Account account, out decimal totalTransport, out int totalAdmissionsPatient, out int totalAdmissionsGuardian, out int totalDischargesPatient, out int totalDischargesGuardian, out int totalHospitalTripsPatient, out int totalHospitalTripsGuardian)
        {
            var result = new List<PatientReportItem>();
            decimal tt = 0;

            int tap = 0;
            int tag = 0;
            int tdp = 0;
            int tdg = 0;
            int thp = 0;
            int thg = 0;

            var admissions = _admitRepo.Table.Include(x => x.Hospital).Include(x => x.Account).Include(x => x.Account.Hospital).Where(x => x.AccountId == account.AccountId).ToList();
            var discharges = _dischargeRepo.Table.Include(a => a.Hospital).Include(x => x.Account).Include(x => x.Account.Hospital).Where(x => x.AccountId == account.AccountId).ToList();
            var hospitalTrips = _hospitalTripRepo.Table.Include(x => x.Hospital).Include(x => x.Account).Where(x => x.AccountId == account.AccountId).ToList();

            foreach (var a in admissions)
            {
                string t = "Patient";
                if (!a.Patient) t = "";
                if (a.Gaurdian)
                {
                    if (a.Patient) t = "Patient and Guardian";
                    else t = "Guardian";
                }

                var model = new PatientReportItem()
                {
                    Date = a.AdmittedDate,
                    Description = "Admission for " + t,
                    ExtraInfo = a.Hospital.Name + (String.IsNullOrEmpty(a.DoctorName) ? "" : " - " + a.DoctorName),
                    Cost = CalculateAdmission(a.Account, a.HospitalId)
                };

                if (a.Gaurdian) tag++;
                if (a.Patient) tap++;

                if (a.Patient && a.Gaurdian)
                {
                    model.Cost = model.Cost * 2;
                }

                tt += model.Cost;

                result.Add(model);
            }

            foreach (var a in discharges)
            {
                string t = "Patient";
                if (!a.Patient) t = "";
                if (a.Gaurdian)
                {
                    if (a.Patient) t = "Patient and Guardian";
                    else t = "Gaurdian";
                }

                var model = new PatientReportItem()
                {
                    Date = a.DischargeDate,
                    Description = "Discharge for " + t,
                    ExtraInfo = a.Hospital.Name,
                    Cost = CalculateDischarge(a.Account, a.HospitalId)
                };

                if (a.Gaurdian) tdg++;
                if (a.Patient) tdp++;

                if (a.Patient && a.Gaurdian) model.Cost = model.Cost * 2;

                tt += model.Cost;

                result.Add(model);
            }

            foreach (var a in hospitalTrips)
            {
                bool chargeDouble = account.ChargeDoubleAtDate(a.Date, admissions, discharges);

                string description = a.ReturnTrip ? "Return hospital trip for Patient" : "Single hospital trip for Patient";
                if (chargeDouble)
                {
                    description = description.Replace(" for Patient", "");
                    description = description + " for Patient and Guardian";
                    thp++;
                    thg++;
                }
                else
                {
                    if (account.Gaurdian)
                    {
                        if (a.ReturnTrip)
                            description = "Return hospital trip for Guardian";
                        else description = "Single hospital trip for Guardian";

                        thg++;
                    }
                    else
                    {
                        thp++;
                    }
                }

                var model = new PatientReportItem()
                {
                    Date = a.Date,
                    Description = description,
                    ExtraInfo = a.Hospital.Name,
                    Cost = CalculateHospitalTrip(a.Account, a.HospitalId, a.ReturnTrip)
                };

                if (chargeDouble) model.Cost = model.Cost * 2;

                tt += model.Cost;

                result.Add(model);
            }

            result = result.OrderBy(x => x.Date).ToList();

            totalTransport = tt;

            totalAdmissionsGuardian = tag;
            totalAdmissionsPatient = tap;
            totalDischargesGuardian = tdg;
            totalDischargesPatient = tdp;
            totalHospitalTripsGuardian = thg;
            totalHospitalTripsPatient = thp;

            return result;
        }

        private decimal CalculateDischarge(Account account, int hospitalId)
        {
            if (account.RegionId == 1)
            {
                if (hospitalId == 24 || hospitalId == 2 || hospitalId == 44) return 548.48m;
                else return 415.35m;
            }
            else
            {
                if (hospitalId == 6) return 1011.75m;

                if (new int[] { 2, 3, 4, 5, 37, 24, 39, 44 }.Contains(hospitalId)) return 548.48m;
                else return 374.88m;
            }
        }

        private decimal CalculateAdmission(Account account, int hospitalId)
        {
            if (account.RegionId == 1)
            {
                if (hospitalId == 2 || hospitalId == 44) return 548.48m;
                else return 415.35m;
            }
            else
            {
                if (hospitalId == 6) return 1011.75m;

                if (new int[] { 2, 3, 4, 5, 37, 24, 39, 44 }.Contains(hospitalId)) return 548.48m;
                else return 374.88m;
            }
        }

        private decimal CalculateHospitalTrip(Account account, int hospitalId, bool returnTrip)
        {
            if (account.RegionId == 1)
            {
                if (new int[] { 2, 3, 4, 5, 37, 44 }.Contains(hospitalId))
                {
                    if (returnTrip) return 1096.95m;
                    else return 548.48m;
                }

                if (hospitalId == 6 || hospitalId == 39)
                {
                    if (returnTrip) return 2023.50m;
                    else return 1011.75m;
                }

                if (returnTrip) return 668.82m;
                else return 334.41m;
            }
            else
            {
                if (new int[] { 2, 3, 4, 5, 37, 44 }.Contains(hospitalId))
                {
                    if (returnTrip) return 1096.95m;
                    else return 548.48m;
                }

                if (hospitalId == 6 || hospitalId == 39)
                {
                    if (returnTrip) return 2023.50m;
                    else return 1011.75m;
                }

                if (returnTrip) return 621.96m;
                else return 310.98m;
            }

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

    public class PatientReportItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ExtraInfo { get; set; }
        public decimal Cost { get; set; }
    }

    public class FinancialReportItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class HospitalTripReportItem
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Qty { get; set; }
    }

    public class HospitalReportItem
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Qty { get; set; }
    }

    public class AdmissionDischargeReportItem
    {
        public DateTime Date { get; set; }
        public bool IsAdmission { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
    }

    public class AccommodationReportItem
    {
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }

    }

    public class AdmissionReportItem
    {
        public DateTime Date { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
    }

    public class BatchReportItem
    {
        public string AccountId { get; set; }
        public string NameSurname { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string ExtraInfo { get; set; }
        public decimal Amount { get; set; }
    }

    public class MinifiedItem
    {
        public string AccountId { get; set; }
        public string AuthNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public string AdmissionDate { get; set; }
        public string DischargedDate { get; set; }
    }
}
