using BlazorApp.Attributes;

namespace BlazorApp.Models
{
	[Table("books")]
	public class Book : DatabaseModel<Book>
	{
		[SqlItem("id", "SERIAL PRIMARY KEY")]
		public int Id { get; set; }

		[SqlItem("title", "TEXT NOT NULL")]
		public string Title { get; set; }

		[SqlItem("author", "TEXT NOT NULL")]
		public string Author { get; set; }

		[SqlItem("genre_id", "TEXT NOT NULL")]
		public string GenreId { get; set; }

		[SqlItem("length", "INT NOT NULL")]
		public int Length { get; set; }

		[SqlItem("price", "DOUBLE NOT NULL")]
		public double Price { get; set; }

		[SqlItem("is_new_condition", "BOOL NOT NULL")]
		public bool IsNewCondition { get; set; }

		[SqlItem("year_of_print", "INT NOT NULL")]
		public int YearOfPrint { get; set; }

		[SqlItem("stock", "INT NOT NULL")]
		public int Stock { get; set; }

		[SqlItem("seller_id", "INT NOT NULL")]
		public int SellerId { get; set; }

		[SqlItem("category_id", "INT NOT NULL")]
		public int CategoryId { get; set; }

		[SqlItem("description", "STRING NOT NULL")]
		public string Description { get; set; }

		public bool IsInStock
		{
			get
			{
				return Stock > 0;
			}
		}

		private static List<Book> _books = new List<Book>();
		
		public static List<Book> SearchBooks(string searchInput)
		{
			if (string.IsNullOrWhiteSpace(searchInput))
			{
				return _books.ToList();
			}

			return _books
				.Where(b => b.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
						|| b.Author.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
				.ToList();
		}
	}
}