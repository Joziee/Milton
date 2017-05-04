using Milton.Database.Models.Business;
using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Finance
{
    public class TransportRate : BaseEntity
    {
        public int TransportRateId { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public string ServiceTypeEnum { get; set; }
        public ServiceType ServiceType
        {
            get { return (ServiceType)Enum.Parse(typeof(ServiceType), this.ServiceTypeEnum); }
            set { this.ServiceTypeEnum = value.ToString(); }
        }
        public decimal Amount { get; set; }
    }
}
