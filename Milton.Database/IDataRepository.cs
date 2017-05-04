using Milton.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database
{
	public partial interface IDataRepository<T> where T : BaseEntity
	{
		T GetById(object id);
		void Insert(T entity);
		void Update(T entity);
		void Delete(T entity);
		IQueryable<T> Table { get; }
	}
}
