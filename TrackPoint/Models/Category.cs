using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
	/**
	 * This class represents a catagory of assets
	 */ 
	public class Categroy
	{
		[Key]
		public int CategoryId { get; set; }
		public string? Name { get; set; }
		public string? Abbreviation { get; set; }
	}
}
