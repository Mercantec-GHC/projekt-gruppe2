using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
	public class Order
	{
		[Key]
		public int OrderId { get; set; }
		public Book BookPurchased { get; set; } 
		public int AmountBooksPurchased { get; set; }
		public DateTime? TimeOfPurchase { get; set; }
		public User Seller { get; set; }
	}
}
