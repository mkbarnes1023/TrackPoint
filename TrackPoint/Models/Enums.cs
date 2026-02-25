namespace TrackPoint.Models
{
    /**
     * This class stores all of the Enums used throughout the project
     */
    public static class Enums
    {
		// Statuses pulled from Excel sheet provided by LSNF
		public enum AssetStatus
		{
			InUse,				// Loaned to somebody, Unavailable
			InStorage,			// Available
			UnderMaintenance,	// Broken and Unavailable
			Retired,			// Not in use anymore, Unavailable
			PendingDeployment,	// Not in circulation yet, Unavailable
			Lost,				// Lost, Unavailable
			Transfered,			// Unclear whether this means in transit or just finished transit
			NeedsReplacement,	// Needs replacement, unclear whether it is available or not
			OnHold,				// Somebody has requested this asset, Unavailable
			Returned,			// Sent back to distributer, Unavailable
			Stationary			// Communal, doesn't get loaned out, Unavailable
		}

        // Event types for Transfer Log entries
        // TODO: Confirm if these are correct and if we need more
        public enum eventType
        {
            Creation,
            BorrowerTransfer,
            StatusChange,
            Archival,
            Deletion
        }
    }
}
