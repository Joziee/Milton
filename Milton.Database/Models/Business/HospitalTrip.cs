using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class HospitalTrip : BaseEntity
    {
        public HospitalTrip()
        {
            this.Date = DateTime.Now;
        }

        public int HospitalTripId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public bool ReturnTrip { get; set; }
        public DateTime Date { get; set; }
        public bool Sent { get; set; }
    }
}
