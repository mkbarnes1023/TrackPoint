using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
	/**
	 * This class represents a category of assets
	 */ 
	public class Category
	{
		[Key]
		public int CategoryId { get; set; }
		public string? Name { get; set; }
		public string? Abbreviation { get; set; }
	}
}
