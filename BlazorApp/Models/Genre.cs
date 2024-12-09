using BlazorApp.Attributes;

namespace BlazorApp.Models
{
	[Table("genre")]
	public class Genre : DatabaseModel<Genre>
	{
		[SqlItem("id", "SERIAL PRIMARY KEY")]
		public int Id { get; set; }

		[SqlItem("name", "TEXT NOT NULL")]
		public string Name { get; set; }
	}
}
