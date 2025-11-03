using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
	/**
	 * This class represents an asset in the system
	 */ 
	public class Asset
	{
		/**
		 * Properties pulled from the Excel sheet provided by LSNF
		 * Other properties that have been discussed:
		 *  - Expiration date
		 *  - Warrenty Information
		 *  - Whether the asset is part of a Outreach Kit (Will likely need its own class with a foreign key)
		 *  - 
		 */
		[Key]
		// Client's asset tag is primary key for now, but maybe it should be a int for consistancy
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
    public DateTime TransferDate { get; set; } // Date the asset was last transfered, added by Matthew
    public string? Notes { get; set; }
    public virtual ICollection<AuditTrail> AuditTrail { get; set; } = new List<AuditTrail>(); // Allows for navigation through the asset's Audit Trail

    public Asset()
		{

		}
        // Constructor for the purpose of creating sample data
        public Asset(string assetTag, string? make, string? model, string? category, string? location, string? issuedTo, AssetStatus status, DateTime transferDate, string? notes)
		{
			AssetTag = assetTag;
			Make = make;
			Model = model;
			Category = category;
			Location = location;
			IssuedTo = issuedTo;
			Status = status;
            TransferDate = transferDate;
            Notes = notes;
		}

		// Sample data for testing purposes. Contains no foreign keys, just strings.
		public static IEnumerable<Asset> SampleAssets = new List<Asset>()
		{
			// Sample data generated with copilot, thanks copilot
			new Asset("A001", "Dell", "XPS 13", "Laptop", "Main Office", "John Doe", AssetStatus.InUse, new DateTime(2025, 10, 1, 9, 0, 0), "Company laptop"),
			new Asset("A002", "Apple", "iPhone 12", "Phone", "Main Office", "Jane Smith", AssetStatus.InUse, new DateTime(2025, 10, 1, 10, 0, 0), "Company phone"),
			new Asset("A003", "HP", "LaserJet Pro", "Printer", "Main Office", null, AssetStatus.InStorage, new DateTime(2025, 10, 2, 11, 0, 0), "Office printer"),
			new Asset("A004", "Cisco", "RV340", "Router", "Data Center", null, AssetStatus.InStorage, new DateTime(2025, 10, 1, 12, 0, 0), "Network router"),
			new Asset("A005", "Lenovo", "ThinkPad X1", "Laptop", "Main Office", "Alice Johnson", AssetStatus.UnderMaintenance, new DateTime(2025, 10, 2, 13, 0, 0), "Needs screen replacement"),
			new Asset("A006", "Samsung", "Galaxy S21", "Phone", "Main Office", null, AssetStatus.PendingDeployment, new DateTime(2025, 10, 3, 14, 0, 0), "New phone for deployment"),
			new Asset("A007", "Epson", "WorkForce WF-2830", "Printer", "Main Office", null, AssetStatus.Retired, new DateTime(2025, 10, 3, 15, 0, 0), "Old office printer"),
			new Asset("A008", "Netgear", "Nighthawk AX12", "Router", "Data Center", null, AssetStatus.InUse, new DateTime(2025, 10, 1, 16, 0, 0), "Main network router"),
			new Asset("A009", "Microsoft", "Surface Pro 7", "Tablet", "Main Office", "Bob Brown", AssetStatus.InUse, new DateTime(2025, 10, 3, 17, 0, 0), "Company tablet"),
			new Asset("A010", "Google", "Pixel 5", "Phone", "Main Office", null, AssetStatus.Lost, new DateTime(2025, 10, 2, 18, 0, 0), "Lost company phone")
		};

	}
}
