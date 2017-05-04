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
    public class BatchService : IBatchService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.Batch";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.Batch.GetById({0})";
        protected const String KEY_GET_BY_ACCOUNT_ID = "Milton.Cache.Business.Batch.GetByAccountId({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.Batch.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Batch> _repo;

        #endregion

        #region Constructor

        public BatchService(ICacheManager cache, IDataRepository<Batch> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Batch model)
        {
            if (model == null) throw new ArgumentNullException("batch");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Batch model)
        {
            if (model == null) throw new ArgumentNullException("batch");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Batch model)
        {
            if (model == null) throw new ArgumentNullException("batch");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Batch GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Batch>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Accounts).Include(x => x.BorderTrips).FirstOrDefault(x => x.BatchId == id);

                //Return
                return result;
            });
        }

        public List<Batch> GetByAccountId(int accountId)
        {
            String key = String.Format(KEY_GET_BY_ACCOUNT_ID, accountId);
            return _cache.Get<List<Batch>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(a => a.Accounts).Where(x => x.Accounts.Any(a => a.AccountId == accountId));

                //Return
                return result.ToList<Batch>();
            });
        }

        public List<Batch> GetAll()
        {
            return _cache.Get<List<Batch>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Accounts).Include(x => x.BorderTrips);

                result = result.OrderByDescending(x => x.SubmissionDate);

                //Return
                return result.ToList<Batch>();
            });
        }
    }
}
