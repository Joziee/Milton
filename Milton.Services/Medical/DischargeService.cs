using Milton.Database;
using Milton.Database.Models.Medical;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Milton.Services.Medical
{
    public class DischargeService : IDischargeService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.Discharge";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.Discharge.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Medical.Discharge.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.Discharge.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Discharge> _repo;

        #endregion

        #region Constructor

        public DischargeService(ICacheManager cache, IDataRepository<Discharge> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Discharge model)
        {
            if (model == null) throw new ArgumentNullException("discharge");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Discharge model)
        {
            if (model == null) throw new ArgumentNullException("discharge");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Discharge model)
        {
            if (model == null) throw new ArgumentNullException("discharge");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Discharge GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Discharge>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.DischargeId == id);

                //Return
                return result;
            });
        }

        public List<Discharge> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<Discharge>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(a => a.Hospital).Where(x => x.AccountId == accountId);

                result = result.OrderByDescending(x => x.DischargeDate);

                //Return
                return result.ToList<Discharge>();
            });
        }

        public List<Discharge> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<Discharge>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account).Include(x => x.Hospital);

                //Return
                return result.ToList<Discharge>();
            });
        }
    }
}
