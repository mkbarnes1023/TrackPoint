
using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
	/**
	 * This class represents an asset in the system
	 */ 
	public class Asset
	{
		[Key]
		public string AssetTag { get; set; }
		public string? Make { get; set; }
		public string? Model { get; set; }
		// Catagory will be made a foreign key later, so new categories can be added as needed.
		public string? Category { get; set; }
		// Location will be made a foreign key later, so new locations can be added as needed.
		public string? Location { get; set; }
		// IssuedTo will be made a foreign key later, so information about the user can be accessed from the asset
		public string? IssuedTo { get; set; }
		// Statuses pulled from Excel sheet provided by LSNF
		public enum AssetStatus
		{
			InUse,
			InStorage,
			UnderMaintenance,
			Retired,
			PendingDeployment,
			Lost,
			Transfered,
			NeedsReplacement,
			OnHold,
			Returned
		}
		public AssetStatus Status { get; set; }
		public string? Notes { get; set; }

		public Asset(string assetTag, string? make, string? model, string? category, string? location, string? issuedTo, AssetStatus status, string? notes)
		{
			AssetTag = assetTag;
			Make = make;
			Model = model;
			Category = category;
			Location = location;
			IssuedTo = issuedTo;
			Status = status;
			Notes = notes;
		}

		// Sample data for testing purposes. Contains no foreign keys, just strings.
		public static IEnumerable<Asset> SampleAssets = new List<Asset>()
		{
			// Sample data generated with copilot, thanks copilot
			new Asset("A001", "Dell", "XPS 13", "Laptop", "Main Office", "John Doe", AssetStatus.InUse, "Company laptop"),
			new Asset("A002", "Apple", "iPhone 12", "Phone", "Main Office", "Jane Smith", AssetStatus.InUse, "Company phone"),
			new Asset("A003", "HP", "LaserJet Pro", "Printer", "Main Office", null, AssetStatus.InStorage, "Office printer"),
			new Asset("A004", "Cisco", "RV340", "Router", "Data Center", null, AssetStatus.InStorage, "Network router"),
			new Asset("A005", "Lenovo", "ThinkPad X1", "Laptop", "Main Office", "Alice Johnson", AssetStatus.UnderMaintenance, "Needs screen replacement"),
			new Asset("A006", "Samsung", "Galaxy S21", "Phone", "Main Office", null, AssetStatus.PendingDeployment, "New phone for deployment"),
			new Asset("A007", "Epson", "WorkForce WF-2830", "Printer", "Main Office", null, AssetStatus.Retired, "Old office printer"),
			new Asset("A008", "Netgear", "Nighthawk AX12", "Router", "Data Center", null, AssetStatus.InUse, "Main network router"),
			new Asset("A009", "Microsoft", "Surface Pro 7", "Tablet", "Main Office", "Bob Brown", AssetStatus.InUse, "Company tablet"),
			new Asset("A010", "Google", "Pixel 5", "Phone", "Main Office", null, AssetStatus.Lost, "Lost company phone")
		};

	}
}
