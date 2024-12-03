using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
	public class Genre
	{
		[Key]
		public int GenreId { get; set; }
		public string GenreName { get; set; } = string.Empty;
	}
}
