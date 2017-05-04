using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Finance
{
    public class AccommodationRate : BaseEntity
    {
        public int AccommodationRateId { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public decimal AccommodationAmountChild { get; set; }
        public decimal AccommodationAmountAdult { get; set; }
        public decimal MealAmountChild { get; set; }
        public decimal MealAmountAdult { get; set; }
    }
}
