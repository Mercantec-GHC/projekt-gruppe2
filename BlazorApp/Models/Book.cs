using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
	public class Book
	{
		[Key]
		public int BookId { get; set; }
		public string Title { get; set; } = "";
		public string Author { get; set; } = "";
		public List<Genre>? Genre { get; set; }
		public decimal Price { get; set; }
		public List<string>? Languages { get; set; }
		public List<string>? Category { get; set; }
		public string Condition { get; set; } = "";
		public int AmountInStock { get; set; } = 0;
		public User Seller { get; set; }
		public bool IsInStock
		{
			get { return AmountInStock > 0; }
		}
	}
}
