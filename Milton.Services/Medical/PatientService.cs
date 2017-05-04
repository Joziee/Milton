using Milton.Database;
using Milton.Database.Models.Medical;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Milton.Services.Medical
{
    public class PatientService : IPatientService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.Patient";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.Patient.GetById({0})";
        protected const String KEY_GET_BY_CRITERIA = "Milton.Cache.Medical.Patient.GetByCriteria({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.Patient.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Patient> _repo;

        #endregion

        #region Constructor

        public PatientService(ICacheManager cache, IDataRepository<Patient> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Patient model)
        {
            if (model == null) throw new ArgumentNullException("patient");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Patient model)
        {
            if (model == null) throw new ArgumentNullException("patient");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Patient model)
        {
            if (model == null) throw new ArgumentNullException("patient");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Patient GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Patient>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.PatientId == id);

                //Return
                return result;
            });
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Patient GetByCriteria(string idNumber)
        {
            String key = String.Format(KEY_GET_BY_CRITERIA, idNumber);
            return _cache.Get<Patient>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.IdNumber == idNumber);

                //Return
                return result;
            });
        }

        public List<Patient> GetAll(int? regionId = null)
        {
            String key = String.Format(KEY_GET_ALL, regionId);
            return _cache.Get<List<Patient>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Region);

                if (regionId.HasValue)
                {
                    result = _repo.Table.Include(x => x.Region).Where(x => x.Region.RegionId == regionId.Value);
                }

                //Return
                return result.ToList<Patient>();
            });
        }

        public List<Patient> Report(DateTime fromDate, DateTime toDate)
        {
            //Query
            var result = _repo.Table.Include(x => x.Region).Include(x => x.Hospital);

            //filter dates
            result = result.Where(x => x.DepartureDate > fromDate && x.DepartureDate < toDate);

            //Return
            return result.ToList<Patient>();
        }

        public ReportResult TotalInvoices(DateTime startDate, DateTime endDate)
        {

            ReportResult r = new ReportResult();

            var result = _repo.Table.Include(x => x.Region).Include(x => x.Hospital);

            //filter dates
            var filteredResult = result.Where(x => x.DepartureDate > startDate && x.DepartureDate < endDate).ToList();

            #region Accommodation

            decimal total = 0;
            decimal patientRate = 0;
            decimal gaurdianRate = 0;

            foreach (var item in filteredResult)
            {
                if (item.DateOfBirth == null) continue;
                if (item.Gaurdian && item.GaurdianDateOfBirth == null) continue;

                int patientAge = (DateTime.Now - item.DateOfBirth.Value).Days;
                bool gaurdian = item.Gaurdian;
                int? hospitalId = item.HospitalId;
                int regionId = item.RegionId;

                if (gaurdian)
                {
                    int gaurdianAge = (DateTime.Now - item.GaurdianDateOfBirth.Value).Days;

                    if (gaurdianAge > 0 && gaurdianAge <= 5)
                    {

                    }
                    else if (gaurdianAge > 5 && gaurdianAge <= 10)
                    {
                        //Botswana
                        if (regionId == 1)
                            gaurdianRate = 412.16m;
                        else
                            gaurdianRate = 351.45m;
                    }
                    else
                    {
                        //Botswana
                        if (regionId == 1)
                            gaurdianRate = 824.45m;
                        else
                            gaurdianRate = 702.90m;
                    }
                }

                if (patientAge > 0 && patientAge <= 5)
                {

                }
                else if (patientAge > 5 && patientAge <= 10)
                {
                    //Botswana
                    if (regionId == 1)
                        patientRate = 412.16m;
                    else
                        patientRate = 351.45m;
                }
                else
                {
                    //Botswana
                    if (regionId == 1)
                        patientRate = 824.45m;
                    else
                        patientRate = 702.90m;
                }

                total += item.DaysAccommodation * patientRate;
                total += item.DaysAccommodation * 180m;
                if (gaurdian)
                {
                    total += item.GaurdianDaysAccommodation * gaurdianRate;
                    total += item.GaurdianDaysAccommodation * 180m;
                }

                //admission and discharged rates
                decimal admittedRate = 0;
                if (regionId == 1) admittedRate = 415.35m;
                else admittedRate = 374.88m;

                decimal dischargedRate = 0;
                if (regionId == 1) dischargedRate = 415.35m;
                else dischargedRate = 374.88m;

                total += item.AdmittedTransport * admittedRate;
                total += item.DischargedTransport * dischargedRate;
            }

            r.TotalAccommodation = total;

            #endregion

            #region Transport

            decimal totalTransport = 0;

            foreach (var item in filteredResult)
            {
                int regionId = item.RegionId;
                decimal totalPatient = 0;

                //botswana
                if (regionId == 1)
                {
                    totalPatient = item.NormalTransport * 334.41m;
                    totalPatient += item.AdmittedTransport * 415.35m;
                    totalPatient += item.DischargedTransport * 415.35m;
                }
                else
                {
                    totalPatient = item.NormalTransport * 310.98m;
                    totalPatient += item.AdmittedTransport * 374.88m;
                    totalPatient += item.DischargedTransport * 374.88m;
                }

                totalTransport += totalPatient;
            }

            #endregion

            r.TotalTransport = totalTransport;

            return r;
        }

        public ReportResult TotalYearToDateInvoices()
        {
            DateTime now = DateTime.Now;
            ReportResult r = new ReportResult();
            return r;
        }

        public ReportResult TotalMonthToDateCommissionPayable()
        {
            DateTime now = DateTime.Now;
            ReportResult r = new ReportResult();
            return r;
        }

        public ReportResult TotalYearToDateCommissionPayable()
        {
            DateTime now = DateTime.Now;
            ReportResult r = new ReportResult();
            return r;
        }
    }

    public class ReportResult
    {
        public decimal TotalTransport { get; set; }
        public decimal TotalAccommodation { get; set; }
    }
}
