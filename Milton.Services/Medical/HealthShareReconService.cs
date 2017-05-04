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
    public class HealthShareReconService : IHealthShareReconService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.HealthShareRecon";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.HealthShareRecon.GetById({0})";
        protected const String KEY_GET_BY_CRITERIA = "Milton.Cache.Medical.HealthShareRecon.GetByCriteria({0},{1},{2})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.HealthShareRecon.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<HealthShareRecon> _repo;

        #endregion

        #region Constructor

        public HealthShareReconService(ICacheManager cache, IDataRepository<HealthShareRecon> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(HealthShareRecon model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(HealthShareRecon model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(HealthShareRecon model)
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
        public HealthShareRecon GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<HealthShareRecon>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.HealthShareReconId == id);

                //Return
                return result;
            });
        }

        public HealthShareRecon GetByCriteria(DateTime invoiceDate, string idNumber, string invoiceNumber, double amount)
        {
            String key = String.Format(KEY_GET_BY_CRITERIA, invoiceDate, idNumber, invoiceNumber, amount);
            return _cache.Get<HealthShareRecon>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.Where(x => x.InvoiceDate == invoiceDate && x.IdNumber == idNumber && x.InvoiceNumber == invoiceNumber && x.Total == amount);

                //Return
                return result.FirstOrDefault();
            });
        }

        public List<HealthShareRecon> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<HealthShareRecon>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<HealthShareRecon>();
            });
        }
    }
}
