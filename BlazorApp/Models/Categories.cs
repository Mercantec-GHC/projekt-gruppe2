using BlazorApp.Attributes;

namespace BlazorApp.Models
{
    public class Categories : DatabaseModel<Categories>
    {
        [SqlItem("id", "SERIAL PRIMARY KEY")]
        public int Id { get; set; }

        [SqlItem("name", "TEXT NOT NULL")]
        public string Name { get; set; }
    }
}
