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
    public class RegionService : IRegionService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Business.Region";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Business.Region.GetById({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Business.Region.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Region> _repo;

        #endregion

        #region Constructor

        public RegionService(ICacheManager cache, IDataRepository<Region> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Region model)
        {
            if (model == null) throw new ArgumentNullException("region");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Region model)
        {
            if (model == null) throw new ArgumentNullException("region");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Region model)
        {
            if (model == null) throw new ArgumentNullException("region");

            _repo.Delete(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Region GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Region>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.RegionId == id);

                //Return
                return result;
            });
        }

        public List<Region> GetAll()
        {
            return _cache.Get<List<Region>>(KEY_GET_ALL, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<Region>();
            });
        }
    }
}
