using BlazorApp.Attributes;
using BlazorApp.Service;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace BlazorApp.Models
{
    public abstract class DatabaseModel<T>
    {
        /// <summary>
        /// A function to get the Table name.
        /// </summary>
        /// <returns>Name of the table.</returns>
        private static string GetTableName()
        {
            string tableName = typeof(T).Name.ToLower();
            var classAttributes = typeof(T).GetCustomAttributes(false);
            var tableAttribute = classAttributes.OfType<Table>().FirstOrDefault();
			// Kontrollere om modelens klass har en attribute af Table og aflevere dens navn.
            // Ellers afleveres klassens navn som lowercase.
			if (tableAttribute != null)
            {
                tableName = ((Table)tableAttribute).TableName;
            }
            return tableName;
        }

		/// <summary>
		/// Format a string for use in SQL queries.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>Formated string to be used in SQL queries.</returns>
		private static string FormatSql(object input)
        {
            bool isNumeric = input is int ||
                            input is double ||
                            input is float ||
                            input is long ||
                            input is decimal;

            // Hvis inputet er string, sættes ' rundt om det, og hvis det er null, afleveres NULL som string.
            return ((isNumeric || input is bool) ? $"{input}" : (input is null ? "NULL" : $"'{input}'"));
        }

        /// <summary>
        /// Create a copy the item.
        /// </summary>
        /// <returns></returns>
        public T Copy()
        {
            var item = (T)Activator.CreateInstance(typeof(T));
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                
                property.SetValue(item, property.GetValue(this));
            }

            return item;
        }

		/// <summary>
		/// Saves the changes made to this item.
		/// If the "PRIMARY KEY" does not exist in the table, it inserts it into the table.
		/// </summary>
		/// <returns>A new instance of the item with the updated info.</returns>
		public async Task<T> Commit()
        {
            string tableName = GetTableName();
            string keys = "";
            string insert = "";
            string values = "";
            string update = "";

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var primaryKey = "";
            var primaryValue = "";

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var sqlAttribute = attributes.OfType<SqlItem>().FirstOrDefault();
                if (sqlAttribute != null)
                {
                    var sqlItem = (SqlItem)sqlAttribute;
                    if (sqlItem.Sql.Contains("PRIMARY KEY"))
                    {
                        primaryKey = sqlItem.Name;
                        primaryValue = FormatSql(property.GetValue(this));

                        if (keys.Length > 0) keys += ", ";
                        keys += $"{sqlItem.Name}";
                    }
                    if (sqlItem.Sql.Contains("UNIQUE"))
                    {
                        if (keys.Length > 0) keys += ", ";
                        keys += $"{sqlItem.Name}";
                    }
                    if (!sqlItem.Sql.Contains("SERIAL"))
                    {
                        if (insert.Length > 0) insert += ", ";
                        if (values.Length > 0) values += ", ";
                        if (update.Length > 0) update += ", ";

                        var value = FormatSql(property.GetValue(this));

                        insert += $"{sqlItem.Name}";
                        values += $"{value}";
                        update += $"{sqlItem.Name}={value}";
                    }
                }
            }

			// Bygger en funtion i SQL, til at håndtere INSERT INTO,
			// hvis UPDATE ikke lykkedes.
            // Til sidst aflevere værdien af dens PRIMARY KEY, da SELECT ikke kan bruges her.
			string query = $"SET datestyle = DMY;"
                + $"DO $$"
                + $" DECLARE r RECORD;"
                + $" BEGIN"
                + $" UPDATE {tableName} SET {update}"
                + $" WHERE {primaryKey} = {primaryValue}"
                + $" RETURNING {primaryKey} INTO r;"
                + $" IF FOUND"
                + $" THEN RAISE NOTICE '%', r.{primaryKey};"
                + $" ELSE"
                + $" INSERT INTO {tableName}({insert})"
                + $" VALUES({values})"
                + $" RETURNING {primaryKey} INTO r;"
                + $" RAISE NOTICE '%', r.{primaryKey};"
                + $" END IF;"
                + $" END $$;";

            using (var connection = DBService.Instance.GetConnection())
            {
				// Sætter primaryValue til det Notice som SQL aflevere
				connection.Notice += (sender, e) => primaryValue = e.Notice.MessageText;

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {

                }
            }
            // Til sidst laves et SQL opslag og aflevere den første værdi som matcher, ellers default.
            ModelList<T> userList = await QueryBy((primaryKey, primaryValue));
            return userList.FirstOrDefault();
        }

		/// <summary>
		/// Queries a list of all items in the Table.
		/// </summary>
		/// <returns>A ModelList of all items in the Table.</returns>
		public static async Task<ModelList<T>> QueryAll()
        {
            return await QueryBy();
        }

		/// <summary>
		/// Queries a list of all items in the Table where all parameters matches.
		/// </summary>
		/// <param name="parameters">A key and a value to search for.</param>
		/// <returns>A ModelList of items in the Table, matching the parameters.</returns>
		public static async Task<ModelList<T>> QueryBy(params (string Key, object Value)[] parameters)
        {
            return await QueryBy("AND", parameters);
        }

		/// <summary>
		/// Queries a list of all items in the Table where parameters matches. Can use "AND" and "OR"
		/// </summary>
		/// <param name="paramAndOr"></param>
		/// <param name="parameters"></param>
		/// <returns>A ModelList of items in the Table, matching the parameters.</returns>
		public static async Task<ModelList<T>> QueryBy(string paramAndOr, params (string Key, object Value)[] parameters)
        {
            var results = new ModelList<T>();

            string tableName = GetTableName();
            string queryParams = "";

            int paramCount = 0;
            foreach (var param in parameters)
            {
                if (paramCount == 0) queryParams += " WHERE";
                else queryParams += $" {paramAndOr}";

                queryParams += $" {param.Key}={FormatSql(param.Value)}";
                paramCount++;
            }

            string query = $"SELECT * FROM {tableName}{queryParams};";

            using (var connection = DBService.Instance.GetConnection())
            using (var command = new NpgsqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    // Laver en instance a T, den item som bliver brugt.
                    var item = (T)Activator.CreateInstance(typeof(T));

                    // Laver en liste over all værdier som er sat til Public og Instance.
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        var attributes = property.GetCustomAttributes(false);
                        var sqlAttribute = attributes.OfType<SqlItem>().FirstOrDefault();
                        if (sqlAttribute != null)
                        {
                            var sqlItem = (SqlItem)sqlAttribute;
                            if (reader.HasColumn(sqlItem.Name))
                            {
                                var value = reader[sqlItem.Name];
                                if (value != DBNull.Value)
								{
									property.SetValue(item, value);
                                }
                            }
                        }
                    }
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates the tabel in the database.
        /// </summary>
        /// <returns></returns>
        public static async Task BuildTable()
        {
            string tableName = GetTableName();
            string tableColumns = "";

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int propertyCount = 0;
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var sqlAttribute = attributes.OfType<SqlItem>().FirstOrDefault();
                if (sqlAttribute != null)
                {
                    var sqlItem = (SqlItem)sqlAttribute;
                    if (propertyCount > 0) tableColumns += ", ";
                    tableColumns += $"{sqlItem.Name} {sqlItem.Sql}";
                    propertyCount++;
                }
            }

            string query = $"CREATE TABLE IF NOT EXISTS {tableName}({tableColumns});";

            using (var connection = DBService.Instance.GetConnection())
            using (var command = new NpgsqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {

            }
        }
    }

    public class ModelList<T> : List<T>
    {
        public T? FirstOrDefault()
        {
            if (this.Count == 0)
            {
                return default(T);
            }
            return this[0];
        }
    }

    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
