using System;
namespace DummyCrudApi.Models
{
	public class Order : ModelBase
	{
		public long Id { get; set; }
		public string CustomerId { get; set; }
		public long EmployeeId { get; set; }
		public string OrderDate { get; set; }
		public string RequiredDate { get; set; }
		public string ShippedDate { get; set; }
		public long ShipVia { get; set; }
		public decimal Freight { get; set; }
		public string ShipName { get; set; }
		public string ShipAddress { get; set; }
		public string ShipCity { get; set; }
		public string ShipRegion { get; set; }
		public string ShipPostalCode { get; set; }
		public string ShipCountry { get; set; }
	}
}
