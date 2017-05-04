using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Medical
{
    public class Admit : BaseEntity
    {
        public Admit()
        {
            this.CreatedOnUtc = DateTime.Now;
        }

        public int AdmitId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public DateTime AdmittedDate { get; set; }
        public bool Patient { get; set; }
        public bool Gaurdian { get; set; }
        public string DoctorName { get; set; }
        public bool Sent { get; set; }
    }
}
