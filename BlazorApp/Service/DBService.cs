using Npgsql;
namespace BlazorApp.Service
{
	public class DBService
	{
		private readonly string _dbConnectionString;

		public DBService(string dbConnectionString)
		{
			_dbConnectionString = dbConnectionString;
		}

		public NpgsqlConnection GetConnection()
		{
			var connection = new NpgsqlConnection(_dbConnectionString);
			connection.Open();
			return connection;
		}
	}
}
