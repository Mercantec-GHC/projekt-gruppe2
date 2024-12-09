using BlazorApp.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace BlazorApp
{
	public class GenreRepository
	{
		private static List<Genre> genres = new List<Genre>()
		{
			//new Genre { GenreId = 1, GenreName = "Architecture" },
			//new Genre { GenreId = 2, GenreName = "Art" },
			//new Genre { GenreId = 3, GenreName = "Biography & Autobiography" },
			//new Genre { GenreId = 4, GenreName = "Body, Mind & Spirit" },
			//new Genre { GenreId = 5, GenreName = "Business & Economics" },
			//new Genre { GenreId = 6, GenreName = "Comics & Graphic Novels" },
			//new Genre { GenreId = 7, GenreName = "Computers & Technologies" }
		};

		public static List<Genre> GetGenreList()
		{
			return genres;
		}
	}
}
