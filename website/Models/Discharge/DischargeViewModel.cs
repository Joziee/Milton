using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Discharge
{
    public class DischargeViewModel
    {
        public int DischargeId { get; set; }
        public int AccountId { get; set; }
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public DateTime DischargeDate { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
    }
}