using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
	/**
	 * This class represents a location where an asset is stored or assigned
	 */
	public class Location
	{
		[Key]
		public int LocationId { get; set; }
		public string? Name { get; set; }
		public string? Abbreviation { get; set; }
	}
}
