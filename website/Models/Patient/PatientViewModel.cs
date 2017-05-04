using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Patient
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int RegionId { get; set; }
        public bool Gaurdian { get; set; }
        public string GaurdianName { get; set; }
        public string GaurdianSurname { get; set; }
        public string GaurdianIdNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string GaurdianDateOfBirth { get; set; }
    }
}