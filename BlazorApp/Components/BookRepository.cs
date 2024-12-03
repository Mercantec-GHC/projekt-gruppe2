using BlazorApp.Models;
using Microsoft.AspNetCore.Hosting.Server;

namespace BlazorApp.Components
{
	public static class BooksRepository
	{
		private static List<Book> books = new List<Book>()
			{
            // Architecture
            new Book
			{
				BookId = 1,
				Title = "The Architecture of Happiness",
				Author = "Alain de Botton",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Architecture").ToList(),
				Price = 15.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 1
			},
			new Book
			{
				BookId = 2,
				Title = "A Pattern Language: Towns, Buildings, Construction",
				Author = "Christopher Alexander",
				Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Architecture").ToList(),
				Price = 35.00m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Hardcover", "e-book" },
				Condition = "New",
				AmountInStock = 1
			},

            // Art
            new Book
			{
				BookId = 3,
				Title = "The Story of Art",
				Author = "E.H. Gombrich",
				Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Art").ToList(),
				Price = 25.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 0
			},
			new Book
			{
				BookId = 4,
				Title = "Ways of Seeing",
				Author = "John Berger",
				Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Art").ToList(),
				Price = 12.50m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "Used",
				AmountInStock = 1
			},

            // Biography & Autobiography
            new Book
			{
				BookId = 5,
				Title = "Steve Jobs",
				Author = "Walter Isaacson",
				Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Biography & Autobiography").ToList(),
				Price = 19.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Hardcover" },
				Condition = "New",
				AmountInStock = 0
			},
			new Book
			{
				BookId = 6,
				Title = "Becoming",
				Author = "Michelle Obama",
				Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Biography & Autobiography").ToList(),
				Price = 18.50m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "Used",
				AmountInStock = 0
			},

            // Body, Mind & Spirit
            new Book
			{
				BookId = 7,
				Title = "The Power of Now",
				Author = "Eckhart Tolle",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Body, Mind & Spirit").ToList(),
                Price = 14.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 1
			},
			new Book
			{
				BookId = 9,
				Title = "The Four Agreements",
				Author = "Don Miguel Ruiz",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Body, Mind & Spirit").ToList(),
                Price = 10.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "eBook" },
				Condition = "New",
				AmountInStock = 1
			},

            // Business & Economics
            new Book
			{
				BookId = 10,
				Title = "The Lean Startup",
				Author = "Eric Ries",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Business & Economics").ToList(),
                Price = 18.00m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 0
			},
			new Book
			{
				BookId = 11,
				Title = "Principles: Life and Work",
				Author = "Ray Dalio",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Business & Economics").ToList(),
                Price = 24.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Hardcover" },
				Condition = "New",
				AmountInStock = 1
			},

            // Comics & Graphic Novels
            new Book
			{
				BookId = 12,
				Title = "Watchmen",
				Author = "Alan Moore, Dave Gibbons",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Comics & Graphic Novels").ToList(),
                Price = 19.95m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 0
			},
			new Book
			{
				BookId = 13,
				Title = "Maus",
				Author = "Art Spiegelman",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Comics & Graphic Novels").ToList(),
                Price = 14.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Hardcover" },
				Condition = "Used",
				AmountInStock = 0
			},

            // Computers & Technologies
            new Book
			{
				BookId = 14,
				Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
				Author = "Robert C. Martin",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Computers & Technologies").ToList(),
                Price = 34.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Paperback" },
				Condition = "New",
				AmountInStock = 1
			},
			new Book
			{
				BookId = 15,
				Title = "The Pragmatic Programmer: Your Journey to Mastery",
				Author = "Andrew Hunt, David Thomas",
                Genre = GenreRepository.GetGenreList().Where(g => g.GenreName == "Computers & Technologies").ToList(),
                Price = 39.99m,
				Languages = new List<string> { "English" },
				Category = new List < string > { "Hardcover" },
				Condition = "New",
				AmountInStock = 1
			}
		};

		public static List<Book> GetBooks()
        {
            return books;
		}
        public static List<Book> SearchBooks(string bookFilter)
        {
            return books.Where(b => b.Title.Contains(bookFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
