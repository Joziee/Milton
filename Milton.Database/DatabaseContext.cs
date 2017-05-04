using Milton.Database.Models;
using MySql.Data.Entity;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Reflection;

namespace Milton.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class DatabaseContext : DbContext, IDataContext
	{
		#region Constructor
		public DatabaseContext(DefaultDatabaseInitializer initializer = null)
			: base("name=DatabaseContext")
		{
            this.Database.Connection.ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;

			//Set the database initializer (by default use CreateDatabaseIfNotExist<DatabaseContext>())
			System.Data.Entity.Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());

			this.Configuration.LazyLoadingEnabled = false;
			//this.Database.Log = WriteLog;

			DatabaseConfiguration config = new DatabaseConfiguration();
		}

		public DatabaseContext()
			: base("name=DatabaseContext")
		{
			this.Database.Connection.ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;

			this.Configuration.LazyLoadingEnabled = false;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets the specified entity set from the context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public new IDbSet<T> Set<T>() where T : BaseEntity
		{
			return base.Set<T>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Create the model based on the entity type configuration mappings
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//Look for and add the entity configurations to the model builder
			modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

			//Don't pluaralize table names
			modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

			base.OnModelCreating(modelBuilder);
		}

		public override int SaveChanges()
		{
			return base.SaveChanges();
		}

		/// <summary>
		/// Indicates if the database targeted by the connection string exists
		/// </summary>
		/// <returns>A Boolean object</returns>
		public bool DatabaseExists()
		{
			return this.Database.Exists();
		}

		public void ExecuteSql(String sql)
		{
			this.Database.ExecuteSqlCommand(sql);
		}
		#endregion

	}
}
