using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class DailyReportLog : BaseEntity
    {
        public int DailyReportLogId { get; set; }
        public DateTime LastSent { get; set; }
    }
}
