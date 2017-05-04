using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Medical
{
    public class AdmitDischarge : BaseEntity
    {
        public int AdmitDischargeId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public DateTime AdmittedDate { get; set; }
        public DateTime? DischargedDate { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
    }
}
