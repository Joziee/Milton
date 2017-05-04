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
    public class AdmitService : IAdmitService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.Admit";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.Admit.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Medical.Admit.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.Admit.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Admit> _repo;

        #endregion

        #region Constructor

        public AdmitService(ICacheManager cache, IDataRepository<Admit> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Admit model)
        {
            if (model == null) throw new ArgumentNullException("admit");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Admit model)
        {
            if (model == null) throw new ArgumentNullException("admit");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Admit model)
        {
            if (model == null) throw new ArgumentNullException("admit");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Admit GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Admit>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account).Include(x => x.Hospital).FirstOrDefault(x => x.AdmitId == id);

                //Return
                return result;
            });
        }

        public List<Admit> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<Admit>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(a => a.Hospital).Where(x => x.AccountId == accountId);

                result = result.OrderByDescending(x => x.AdmittedDate);

                //Return
                return result.ToList<Admit>();
            });
        }

        public List<Admit> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<Admit>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Account).Include(x => x.Hospital);

                //Return
                return result.ToList<Admit>();
            });
        }
    }
}
