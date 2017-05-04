using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IRegionService
    {
        void Insert(Region model);
        void Update(Region model);
        void Delete(Region model);

        Region GetById(Int32 id);
        List<Region> GetAll();
    }
}
