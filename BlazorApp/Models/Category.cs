﻿using BlazorApp.Attributes;

namespace BlazorApp.Models
{
	[Table("categories")]
	public class Category : DatabaseModel<Category>
	{
		[SqlItem("id", "SERIAL PRIMARY KEY")]
		public int Id { get; set; }

		[SqlItem("name", "TEXT NOT NULL")]
		public string Name { get; set; }

		public async Task<ModelList<Book>> GetBooksAsync()
		{
			return await Book.QueryBy(("category_id", Id));
		}
	}
}
