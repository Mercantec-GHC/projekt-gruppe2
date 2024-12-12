using BlazorApp.Models;
using Npgsql;
namespace BlazorApp.Service
{
	public class DBService
	{
        public static DBService Instance { get; private set; }

        private readonly string _dbConnectionString;

		public DBService(string dbConnectionString)
		{
			_dbConnectionString = dbConnectionString;
			Instance = this;

            User.BuildTable();
            Category.BuildTable();
			Book.BuildTable();
        }

		public NpgsqlConnection GetConnection()
		{
			var connection = new NpgsqlConnection(_dbConnectionString);
			connection.Open();
			return connection;
		}
	}
}
