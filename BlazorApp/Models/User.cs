using BlazorApp.Attributes;

namespace BlazorApp.Models
{
    [Table("users")]
    public class User : DatabaseModel<User>
    {
        [SqlItem("id", "SERIAL PRIMARY KEY")]
        public int Id { get; set; }

        [SqlItem("first_name", "TEXT NOT NULL")]
        public string FirstName { get; set; }

        [SqlItem("last_name", "TEXT NOT NULL")]
        public string LastName { get; set; }

        [SqlItem("email", "TEXT NOT NULL UNIQUE")]
        public string Email { get; set; }

        [SqlItem("username", "TEXT NOT NULL UNIQUE")]
        public string Username { get; set; }

        [SqlItem("password", "TEXT NOT NULL")]
        public string Password { get; set; }

        [SqlItem("role", "TEXT NOT NULL")]
        public string Role { get; set; }

        [SqlItem("created_at", "DATE NOT NULL")]
        public DateTime CreatedAt { get; set; }

        public async Task<ModelList<Book>> GetBooks()
        {
            return await Book.QueryBy(("seller_id", Id));
        }
    }
}
