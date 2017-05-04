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
    public class BorderTripService : IBorderTripService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.BorderTrip";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.BorderTrip.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Business.BorderTrip.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.BorderTrip.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<BorderTrip> _repo;

        #endregion

        #region Constructor

        public BorderTripService(ICacheManager cache, IDataRepository<BorderTrip> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(BorderTrip model)
        {
            if (model == null) throw new ArgumentNullException("border_trip");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(BorderTrip model)
        {
            if (model == null) throw new ArgumentNullException("border_trip");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(BorderTrip model)
        {
            if (model == null) throw new ArgumentNullException("border_trip");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public BorderTrip GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<BorderTrip>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account).FirstOrDefault(x => x.BorderTripId == id);

                //Return
                return result;
            });
        }

        public List<BorderTrip> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<BorderTrip>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Where(x => x.AccountId == accountId);

                //Return
                return result.ToList<BorderTrip>();
            });
        }

        public List<BorderTrip> GetAll()
        {
            return _cache.Get<List<BorderTrip>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account);

                //Return
                return result.ToList<BorderTrip>();
            });
        }
    }
}
