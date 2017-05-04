using Milton.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database
{
	public partial class DataRepository<T> : IDataRepository<T> where T : BaseEntity
	{
		#region Fields
		protected readonly IDataContext _db;
		protected IDbSet<T> _set;
		#endregion

		#region Constructor
		/// <summary>
		/// Constrcuts a new instance of a data repository
		/// </summary>
		/// <param name="context"></param>
		public DataRepository(IDataContext context)
		{
			this._db = context;
		}
		#endregion

		/// <summary>
		/// Fetch an entity from the repository by id
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>Returns an entity or null"/></returns>
		public virtual T GetById(object id)
		{
			return this.Entities.Find(id);
		}

		/// <summary>
		/// Insert a new entity into the repository
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Insert(T entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			this.Entities.Add(entity);
			this._db.SaveChanges();
		}

		/// <summary>
		/// Update an existing entity in the repository
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Update(T entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			if (this.Entities.Local.FirstOrDefault(e => e == entity) == null) Entities.Attach(entity);

			Throw(_db).Entry(entity).State = EntityState.Modified;
			this._db.SaveChanges();

		}

		/// <summary>
		/// Deletes an existing entity from the repository
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Delete(T entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			if (this.Entities.Local.FirstOrDefault(e => e == entity) == null) Entities.Attach(entity);
			this.Entities.Remove(entity);
			this._db.SaveChanges();
		}

		/// <summary>
		/// Acces the entire entity table
		/// </summary>
		public virtual IQueryable<T> Table
		{
			get
			{
				return this.Entities;
			}
		}

		/// <summary>
		/// Access the entire set of entities
		/// </summary>
		protected virtual IDbSet<T> Entities
		{
			get
			{
				if (_set == null) _set = _db.Set<T>();
				return _set;
			}
		}

		/// <summary>
		/// Throw the interface to a database context
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private DbContext Throw(IDataContext context)
		{
			return (DbContext)(context as DbContext);
		}
	}
}
