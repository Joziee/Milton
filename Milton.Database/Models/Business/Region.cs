using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class Region : BaseEntity
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
    }
}
