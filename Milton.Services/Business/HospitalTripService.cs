using Milton.Database;
using Milton.Database.Models.Business;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Milton.Services.Business
{
    public class HospitalTripService : IHospitalTripService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.HospitalTrip";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.HospitalTrip.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Business.HospitalTrip.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.HospitalTrip.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<HospitalTrip> _repo;

        #endregion

        #region Constructor

        public HospitalTripService(ICacheManager cache, IDataRepository<HospitalTrip> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(HospitalTrip model)
        {
            if (model == null) throw new ArgumentNullException("hospital_trip");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(HospitalTrip model)
        {
            if (model == null) throw new ArgumentNullException("hospital_trip");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(HospitalTrip model)
        {
            if (model == null) throw new ArgumentNullException("hospital_trip");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public HospitalTrip GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<HospitalTrip>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.HospitalTripId == id);

                //Return
                return result;
            });
        }

        public List<HospitalTrip> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<HospitalTrip>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Hospital).Where(x => x.AccountId == accountId);

                //Return
                return result.ToList<HospitalTrip>();
            });
        }

        public List<HospitalTrip> GetAll()
        {
            return _cache.Get<List<HospitalTrip>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account).Include(a => a.Hospital);

                //Return
                return result.ToList<HospitalTrip>();
            });
        }
    }
}
