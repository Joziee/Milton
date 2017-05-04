using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Medical
{
    public class Hospital : BaseEntity
    {
        public int HospitalId { get; set; }
        public string Name { get; set; }
        public decimal? BotswanaPrice { get; set; }
        public decimal? BotswanaReturnPrice { get; set; }
        public decimal? SwazilandPrice { get; set; }
        public decimal? SwazilandReturnPrice { get; set; }
    }
}
