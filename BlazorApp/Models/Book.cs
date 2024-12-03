using BlazorApp.Attributes;

namespace BlazorApp.Models
{
	[Table("books")]
	public class Book : DatabaseModel
	{

		[SqlItem("id", "SERIAL PRIMARY KEY")]
		public int Id { get; set; }

		[SqlItem("title", "TEXT NOT NULL")]
		public string Title { get; set; }

		[SqlItem("author", "TEXT NOT NULL")]
		public string Author { get; set; }

		[SqlItem("genre", "TEXT NOT NULL")]
		public string PageCount { get; set; }

		[SqlItem("page_count", "INT NOT NULL")]
		public int Pagecount { get; set; }

		[SqlItem("price", "DOUBLE NOT NULL")]
		public double Price { get; set; } 

		[SqlItem("is_new_condition", "INT NOT NULL")]
		public bool IsNewCondition { get; set; }

		[SqlItem("year_of_print", "JASON NOT NULL")]
		public int YearOfPrint { get; set; }

		[SqlItem("stock", "INT NOT NULL")]
		public int Stock { get; set; }

		[SqlItem("seller_id", "INT NOT NULL")]
		public int SellerId { get; set; }

		[SqlItem("description", "DOUBLE NOT NULL")]
		public string Description { get; set; }

		public bool IsInStock {
			get
			{
				return Stock > 0;
			}
		}
	}
}
