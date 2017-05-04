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
    public class ReconService : IReconService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Medical.Recon";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Medical.Recon.GetById({0})";
        protected const String KEY_GET_BY_CRITERIA = "Milton.Cache.Medical.Recon.GetByCriteria({0},{1},{2})";
        protected const String KEY_GET_ALL = "Milton.Cache.Medical.Recon.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Recon> _repo;

        #endregion

        #region Constructor

        public ReconService(ICacheManager cache, IDataRepository<Recon> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Recon model)
        {
            if (model == null) throw new ArgumentNullException("recon");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Recon model)
        {
            if (model == null) throw new ArgumentNullException("recon");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Recon model)
        {
            if (model == null) throw new ArgumentNullException("recon");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Recon GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Recon>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.ReconId == id);

                //Return
                return result;
            });
        }

        public Recon GetByCriteria(DateTime date, string invoiceNumber, decimal amount)
        {
            String key = String.Format(KEY_GET_BY_CRITERIA, date, invoiceNumber, amount);
            return _cache.Get<Recon>(key, 10, () =>
            {
                string invn = invoiceNumber.Replace("-", "").ToLower();
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.Date == date && x.InvoiceNumber.ToLower() == invn && x.Total == amount);

                //Return
                return result;
            });
        }

        public List<Recon> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<Recon>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<Recon>();
            });
        }
    }
}
