using BlazorApp.Attributes;
using BlazorApp.Service;
using Npgsql;
using System.Data;
using System.Reflection;

namespace BlazorApp.Models
{
    public class DatabaseModel
    {
        public static async Task<ModelList<T>> QueryAll<T>()
        {
            return await QueryBy<T>();
        }

        public static async Task<ModelList<T>> QueryBy<T>(params (string Key, object Value)[] parameters)
        {
            var results = new ModelList<T>();

            string tableName = typeof(T).Name.ToLower();
            var classAttributes = typeof(T).GetCustomAttributes(false);
            var tableAttribute = classAttributes.OfType<Table>().FirstOrDefault();
            if (tableAttribute != null)
            {
                tableName = ((Table)tableAttribute).TableName;
            }

            string query = $"SELECT * FROM {tableName}";

            int paramCount = 0;
            foreach (var param in parameters)
            {
                if (paramCount == 0) query += " WHERE";
                else query += " AND";

                // To be changed later!
                string test = (param.Value is string ? "'" : "");
                query += $" {param.Key}={test}{param.Value}{test}";
                paramCount++;
            }

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
