using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IHospitalTripService
    {
        void Insert(HospitalTrip model);
        void Update(HospitalTrip model);
        void Delete(HospitalTrip model);

        HospitalTrip GetById(Int32 id);
        List<HospitalTrip> GetByAccountId(Int32 accountId);
        List<HospitalTrip> GetAll();
    }
}
