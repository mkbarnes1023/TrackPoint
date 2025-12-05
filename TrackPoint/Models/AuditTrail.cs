namespace TrackPoint.Models
{
    public class AuditTrail

    {
        public int AuditTrailId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string IssuedTo { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        //public virtual Asset Asset { get; set; } = null!;
    }
}
