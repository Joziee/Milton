using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.AdmitDischarge
{
    public class AdmitDischargeViewModel
    {
        public int AdmitDischargeId { get; set; }
        public int AccountId { get; set; }
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public DateTime AdmittedDate { get; set; }
        public DateTime? DischargedDate { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
    }
}