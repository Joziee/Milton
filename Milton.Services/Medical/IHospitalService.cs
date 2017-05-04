using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public interface IHospitalService
    {
        void Insert(Hospital model);
        void Update(Hospital model);
        void Delete(Hospital model);

        Hospital GetById(Int32 id);
        List<Hospital> GetAll();
    }
}
