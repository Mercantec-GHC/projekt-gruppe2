using BlazorApp.Attributes;
using BlazorApp.Service;
using Npgsql;
using System.Data;
using System.Reflection;

namespace BlazorApp.Models
{
    public abstract class DatabaseModel<T>
    {
        public static async Task<ModelList<T>> QueryAll()
        {
            return await QueryBy();
        }

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

        public static async Task<ModelList<T>> QueryBy(params (string Key, object Value)[] parameters)
        {
            var results = new ModelList<T>();

            string tableName = GetTableName();
            string queryParams = "";

            int paramCount = 0;
            foreach (var param in parameters)
            {
                if (paramCount == 0) queryParams += " WHERE";
                else queryParams += " AND";

                string quote = (param.Value is string ? "'" : "");
                queryParams += $" {param.Key}={quote}{param.Value}{quote}";
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

            //using (var connection = DBService.Instance.GetConnection())
            //using (var command = new NpgsqlCommand(query, connection))
            //using (var reader = await command.ExecuteReaderAsync())
            //{

            //}

            //Console.WriteLine(query);
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
