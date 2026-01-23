namespace TrackPoint.Models.RepositoryInterfaces
{
    public interface ILocationRepository
    {
		IEnumerable<LocationModel> Locations { get; }
		LocationModel GetLocationByID(string locationId);
		void AddLocation(LocationModel location);
		void RemoveLocation(LocationModel location);
		void RemoveLocationByID(string locationId);
		void UpdateLocation(LocationModel location);
		void Save();
	}
}
