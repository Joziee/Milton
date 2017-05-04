using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IDailyReportLogService
    {
        void Insert(DailyReportLog model);
        void Update(DailyReportLog model);
        void Delete(DailyReportLog model);

        DailyReportLog GetById(Int32 id);
        List<DailyReportLog> GetAll();
    }
}
