using Milton.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database
{
	public interface IDataContext
	{
		IDbSet<T> Set<T>() where T : BaseEntity;
		int SaveChanges();
		bool DatabaseExists();
		void ExecuteSql(String sql);
	}
}
