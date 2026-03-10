namespace TrackPoint.Models.DTOs
{
    public class BorrowerDashboardViewModel
    {
        //mcount the number of assets assinged to the current singed in user

        public int AssignedToUser { get; set; }
        public List<Asset> Assigned { get; set; } = new List<Asset>();

    }
}
