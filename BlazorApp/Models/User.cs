using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string FirstName { get; set; } = string.Empty;
		[Required]
		public string LastName { get; set; } = string.Empty;
		[Required]
		public string Email { get; set; } = string.Empty;
		[Required]
		private string Password { get; } = string.Empty;
		public List<string> OrderHistory { get; }
		public List<Book> Products { get; }


		public bool ValidatePassword(string password)
		{ 
			return Password == password;
		}

	}
}
