using Milton.Database.Models.Business;
using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class Account : BaseEntity
    {
        public Account()
        {
            this.CreatedOnUtc = DateTime.Now;
            this.DepartureDate = null;
            this.AccountClosed = false;
        }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Gaurdian { get; set; }
        public string GaurdianName { get; set; }
        public string GaurdianSurname { get; set; }
        public string GaurdianIdNumber { get; set; }
        public DateTime? GaurdianDateOfBirth { get; set; }
        public string GaurdianAuthNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public bool AccountClosed { get; set; }
        public bool NeedLog { get; set; }
        public string AuthNumber { get; set; }
        public int? HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public ICollection<BorderTrip> BorderTrips { get; set; }
        public ICollection<HospitalTrip> HospitalTrips { get; set; }
        public ICollection<OtherExpense> OtherExpenses { get; set; }
        public ICollection<Admit> Admissions { get; set; }
        public ICollection<Discharge> Discharges { get; set; }
        public ICollection<Batch> Batches { get; set; }
        public bool PatientBegin { get; set; }
        public bool GaurdianBegin { get; set; }
    }
}
