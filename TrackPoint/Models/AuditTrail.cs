namespace TrackPoint.Models
{
    public class AuditTrail
    {
        public string AssetTag { get; set; }
        public string IssuedTo { get; set; }
        public DateTime TransferDate { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
