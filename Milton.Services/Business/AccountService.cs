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
    public class AccountService : IAccountService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.Account";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.Account.GetById({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.Account.GetAll({0},{1})";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Account> _repo;
        protected IDataRepository<Batch> _batchRepo;

        #endregion

        #region Constructor

        public AccountService(ICacheManager cache, IDataRepository<Account> repo, IDataRepository<Batch> batch)
        {
            _cache = cache;
            _repo = repo;
            _batchRepo = batch;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Account model)
        {
            if (model == null) throw new ArgumentNullException("account");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Account model)
        {
            if (model == null) throw new ArgumentNullException("account");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Account model)
        {
            if (model == null) throw new ArgumentNullException("account");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Account GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Account>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.AccountId == id);

                //Return
                return result;
            });
        }

        public List<Account> GetAll(int? regionId = null, bool everyone = true)
        {
            String key = String.Format(KEY_GET_ALL, regionId, everyone.ToString());
            return _cache.Get<List<Account>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Include(x => x.Region);

                if (regionId.HasValue)
                {
                    result = result.Where(x => x.RegionId == regionId.Value);
                }

                if (!everyone)
                {
                    result = result.Where(x => !x.AccountClosed);
                }

                //Return
                return result.ToList<Account>();
            });
        }

        public List<Account> GetActive(int? regionId = null)
        {

            //Query
            var accounts = _repo.Table.Include(x => x.Region);

            if (regionId.HasValue)
            {
                accounts = accounts.Where(x => x.RegionId == regionId.Value);
            }

            var batchAccountIds = _batchRepo.Table.SelectMany(x => x.Accounts.Select(a => a.AccountId));

            accounts = accounts.Where(a => !batchAccountIds.Contains(a.AccountId));

            //Return
            return accounts.ToList<Account>();

        }

        public Account SearchAccount(string name, string surname)
        {
            string n = name.ToLower().Trim();
            string s = surname.ToLower().Trim();
            return _repo.Table.FirstOrDefault(x => x.Name.ToLower().Contains(n) && x.Surname.ToLower().Contains(s));
        }
    }
}