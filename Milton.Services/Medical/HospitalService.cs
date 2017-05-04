using Milton.Database;
using Milton.Database.Models.Medical;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Medical
{
    public class HospitalService : IHospitalService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.Hospital";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.Hospital.GetById({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.Hospital.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Hospital> _repo;

        #endregion

        #region Constructor

        public HospitalService(ICacheManager cache, IDataRepository<Hospital> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Hospital model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Hospital model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Hospital model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Hospital GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Hospital>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.HospitalId == id);

                //Return
                return result;
            });
        }

        public List<Hospital> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<Hospital>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Sorting
                result = result.OrderBy(x => x.Name);

                //Return
                return result.ToList<Hospital>();
            });
        }
    }
}
