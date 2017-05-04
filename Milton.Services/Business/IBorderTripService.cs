using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IBorderTripService
    {
        void Insert(BorderTrip model);
        void Update(BorderTrip model);
        void Delete(BorderTrip model);

        BorderTrip GetById(Int32 id);
        List<BorderTrip> GetByAccountId(Int32 accountId);
        List<BorderTrip> GetAll();
    }
}
