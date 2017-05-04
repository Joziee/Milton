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
    public class OtherExpenseService : IOtherExpenseService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.OtherExpense";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.OtherExpense.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Business.OtherExpense.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.OtherExpense.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<OtherExpense> _repo;

        #endregion

        #region Constructor

        public OtherExpenseService(ICacheManager cache, IDataRepository<OtherExpense> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(OtherExpense model)
        {
            if (model == null) throw new ArgumentNullException("other_expense");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(OtherExpense model)
        {
            if (model == null) throw new ArgumentNullException("other_expense");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(OtherExpense model)
        {
            if (model == null) throw new ArgumentNullException("other_expense");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public OtherExpense GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<OtherExpense>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.OtherExpenseId == id);

                //Return
                return result;
            });
        }

        public List<OtherExpense> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<OtherExpense>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(a=>a.Account).Where(x => x.AccountId == accountId);

                //Return
                return result.ToList<OtherExpense>();
            });
        }

        public List<OtherExpense> GetAll()
        {
            return _cache.Get<List<OtherExpense>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<OtherExpense>();
            });
        }
    }
}
