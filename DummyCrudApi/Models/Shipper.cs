using System;
namespace DummyCrudApi.Models
{
	public class Shipper: ModelBase
	{
		public long Id { get; set; }
		public string CompanyName { get; set; }
		public string Phone { get; set; }
	}
}
