using System.Data.Entity;

namespace Milton.Database
{
	/// <summary>
	/// This is the standard DSC Transport database initializer.
	/// </summary>
	/// <remarks>
	/// </remarks>
    public class DefaultDatabaseInitializer : MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>
	{
		/// <summary>
		/// Initialize the database
		/// </summary>
		/// <param name="context"></param>
		public override void InitializeDatabase(DatabaseContext context)
		{
            //Create a new database if it doesn't already exist
            if (!context.Database.Exists())
			{
				//Create the database
				context.Database.Create();
			}

			//if (!context.Database.CompatibleWithModel(false)) return;
		}
	}
}
