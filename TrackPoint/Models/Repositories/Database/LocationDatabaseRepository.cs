using TrackPoint.Data;
using TrackPoint.Models.RepositoryInterfaces;

namespace TrackPoint.Models.Repositories.Local
{
	/**
	 * This repository interacts with the database to manage Locations
    public class LocationDatabaseRepository : ILocationRepository
	{
		public IEnumerable<LocationModel> Locations { get; }
		public LocationModel GetLocationByID(string locationId)
		{
			throw new NotImplementedException();
		}
		public void AddLocation(LocationModel location)
		{
			throw new NotImplementedException();
		}
		public void RemoveLocation(LocationModel location)
		{
			throw new NotImplementedException();
		}
		public void RemoveLocationByID(string locationId)
		{
			throw new NotImplementedException();
		}
		public void UpdateLocation(LocationModel location)
		{
			throw new NotImplementedException();
		}
		public void Save()
		{
			throw new NotImplementedException();
		}
	}
}
