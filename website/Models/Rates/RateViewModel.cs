using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Rates
{
    public class RateViewModel
    {
        public int HospitalId { get; set; }
        public string Name { get; set; }
        public decimal? BostwanaPrice { get; set; }
        public decimal? BotswanaReturnPrice { get; set; }
        public decimal? SwazilandPrice { get; set; }
        public decimal? SwazilandReturnPrice { get; set; }
    }
}