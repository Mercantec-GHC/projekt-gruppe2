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

		public async Task<ModelList<Book>> GetBooksAsync()
		{
			ModelList<Book> books = await Book.QueryAll();
			ModelList<Book> booksWithGenre = new ModelList<Book>();
			foreach (Book book in books)
			{
				string[] genres = book.GenreId.Split(",");
				if (genres.Contains($"{Id}"))
				{
					booksWithGenre.Add(book);
				}
			}
			return booksWithGenre;
		}
	}
}
