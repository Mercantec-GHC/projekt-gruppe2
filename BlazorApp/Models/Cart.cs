namespace BlazorApp.Models
{
	public class Cart
	{
		private Dictionary<Book, int> booksInCart = new Dictionary<Book, int>();

		public Dictionary<Book, int> GetBooksInCart()
		{
			return booksInCart;
		}
	}
}
