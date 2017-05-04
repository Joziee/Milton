using Milton.Database;
using Milton.Database.Models.Business;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public class DailyReportLogService : IDailyReportLogService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.DailyReportLog";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.DailyReportLog.GetById({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.DailyReportLog.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<DailyReportLog> _repo;

        #endregion

        #region Constructor

        public DailyReportLogService(ICacheManager cache, IDataRepository<DailyReportLog> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(DailyReportLog model)
        {
            if (model == null) throw new ArgumentNullException("batch");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(DailyReportLog model)
        {
            if (model == null) throw new ArgumentNullException("batch");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(DailyReportLog model)
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
        public DailyReportLog GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<DailyReportLog>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.DailyReportLogId == id);

                //Return
                return result;
            });
        }

        public List<DailyReportLog> GetAll()
        {
            return _cache.Get<List<DailyReportLog>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<DailyReportLog>();
            });
        }
    }
}
