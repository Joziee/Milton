using Milton.Database;
using Milton.Database.Models.Finance;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Finance
{
    public class PaymentService : IPaymentService
    {
        #region Constants

        protected const String PATTERN = "Milton.Cache.Finance.Payment";

        protected const String KEY_GET_BY_ID = "Milton.Cache.Finance.Payment.GetById({0})";
        protected const String KEY_GET_BY_INVOICE_NUMBER = "Milton.Cache.Finance.Payment.GetByInvoiceNumber({0})";
        protected const String KEY_GET_ALL = "Milton.Cache.Finance.Payment.GetAll()";

        #endregion

        #region Fields

        protected ICacheManager _cache;
        protected IDataRepository<Payment> _repo;

        #endregion

        #region Constructor

        public PaymentService(ICacheManager cache, IDataRepository<Payment> repo)
        {
            _cache = cache;
            _repo = repo;
        }

        #endregion

        /// <summary>
        /// Create a new service
        /// </summary>
        /// <param name="service"></param>
        public void Insert(Payment model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Insert(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public void Update(Payment model)
        {
            if (model == null) throw new ArgumentNullException("hospital");

            _repo.Update(model);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Payment model)
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
        public Payment GetById(int id)
        {
            String key = String.Format(KEY_GET_BY_ID, id);
            return _cache.Get<Payment>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.PaymentId == id);

                //Return
                return result;
            });
        }

        public Payment GetByInvoiceNumber(string invoiceNumber)
        {
            String key = String.Format(KEY_GET_BY_INVOICE_NUMBER, invoiceNumber);
            return _cache.Get<Payment>(key, 10, () =>
            {
                //Query
                var result = _repo.Table.FirstOrDefault(x => x.InvoiceNumber.ToLower() == invoiceNumber.ToLower());

                //Return
                return result;
            });
        }

        public List<Payment> GetAll()
        {
            String key = KEY_GET_ALL;
            return _cache.Get<List<Payment>>(key, 10, () =>
            {
                //Query
                var result = _repo.Table;

                //Return
                return result.ToList<Payment>();
            });
        }
    }
}
