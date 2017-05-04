using System.Data.Entity;

namespace Milton.Database
{
    public class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration()
        {
            SetHistoryContext("MySql.Data.MySqlClient", (conn, schema) => new MiltonHistoryContext(conn, schema));
        }
    }
}
