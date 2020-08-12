using System;
namespace DummyCrudApi.Models
{
	public class Category: ModelBase
	{
		public long Id { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
	}
}
