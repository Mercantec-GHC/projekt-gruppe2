namespace BlazorApp.Attributes
{
    public class Table : Attribute
    {
        public string TableName { get; private set; }

        public Table(string tableName)
        {
            TableName = tableName.ToLower();
        }
    }

    public class SqlItem : Attribute
    {
        public string Name { get; private set; }
        public string Sql { get; private set; }

        public SqlItem(string name, string sql)
        {
            Name = name;
            Sql = sql;
        }
    }
}
