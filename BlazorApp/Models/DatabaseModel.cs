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
        private static string GetTableName()
        {
            string tableName = typeof(T).Name.ToLower();
            var classAttributes = typeof(T).GetCustomAttributes(false);
            var tableAttribute = classAttributes.OfType<Table>().FirstOrDefault();
            if (tableAttribute != null)
            {
                tableName = ((Table)tableAttribute).TableName;
            }
            return tableName;
        }

        private static string FormatSql(object input)
        {
            bool isNumeric = input is int ||
                            input is double ||
                            input is float ||
                            input is long ||
                            input is decimal;

            return ((isNumeric || input is bool) ? $"{input}" : (input is null ? "NULL" : $"'{input}'"));
        }

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

        public async Task Commit()
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

            string query = $"SET datestyle = DMY;"
                + $"DO $$"
                + $" BEGIN"
                + $" UPDATE {tableName} SET {update}"
                + $" WHERE {primaryKey} = {primaryValue};"
                + $" IF FOUND"
                + $" THEN RAISE NOTICE 'Row updated';"
                + $" ELSE"
                + $" INSERT INTO {tableName}({insert})"
                + $" VALUES({values});"
                + $" RAISE NOTICE 'Row inserted';"
                + $" END IF;"
                + $" END $$;";

            using (var connection = DBService.Instance.GetConnection())
            using (var command = new NpgsqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {

            }
        }

        public static async Task<ModelList<T>> QueryAll()
        {
            return await QueryBy();
        }

        public static async Task<ModelList<T>> QueryBy(params (string Key, object Value)[] parameters)
        {
            return await QueryBy("AND", parameters);
        }

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
                    var item = (T)Activator.CreateInstance(typeof(T));

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
