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
    public class AdmitDischargeService : IAdmitDischargeService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.AdmitDischarge";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.AdmitDischarge.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Medical.AdmitDischarge.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.AdmitDischarge.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<AdmitDischarge> _repo;

        #endregion

        #region Constructor

        public AdmitDischargeService(ICacheManager cache, IDataRepository<AdmitDischarge> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(AdmitDischarge model)
        {
            if (model == null) throw new ArgumentNullException("admit_discharge");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(AdmitDischarge model)
        {
            if (model == null) throw new ArgumentNullException("admit_discharge");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(AdmitDischarge model)
        {
            if (model == null) throw new ArgumentNullException("admit_discharge");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public AdmitDischarge GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<AdmitDischarge>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.AdmitDischargeId == id);

                //Return
                return result;
            });
        }

        public List<AdmitDischarge> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<AdmitDischarge>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(a => a.Hospital).Where(x => x.AccountId == accountId);

                result = result.OrderByDescending(x => x.DischargedDate.HasValue).ThenBy(x => x.AdmittedDate);

                //Return
                return result.ToList<AdmitDischarge>();
            });
        }

        public List<AdmitDischarge> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<AdmitDischarge>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<AdmitDischarge>();
            });
        }
    }
}
