using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;

namespace Milton.Database.Models.Medical
{
    public class Patient : BaseEntity
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int DaysAccommodation { get; set; }
        public int NormalTransport { get; set; }
        public int AdmittedTransport { get; set; }
        public int DischargedTransport { get; set; }
        public decimal? PharmacyAmount { get; set; }
        public bool Gaurdian { get; set; }
        public string GaurdianName { get; set; }
        public string GaurdianSurname { get; set; }
        public string GaurdianIdNumber { get; set; }
        public int GaurdianDaysAccommodation { get; set; }
        public int GaurdianNormalTransport { get; set; }
        public int GaurdianAdmittedTransport { get; set; }
        public int GaurdianDischargedTransport { get; set; }
        public int RegionId { get; set; }
        public int? HospitalId { get; set; }
        public int? HospitalId2 { get; set; }
        public int? HospitalId3 { get; set; }
        public bool Done { get; set; }


        #region Navigation Properties

        public Region Region { get; set; }
        public Hospital Hospital { get; set; }
        public Hospital Hospital2 { get; set; }
        public Hospital Hospital3 { get; set; }

        #endregion

        public DateTime? DateOfBirth { get; set; }
        public DateTime? GaurdianDateOfBirth { get; set; }

    }
}
