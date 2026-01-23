//using TrackPoint.Data;
//using TrackPoint.Models.RepositoryInterfaces;

//namespace TrackPoint.Models.Repositories.Local
//{
//	/**
//     * This repository interacts with the local database to manage Locations
//     */
//	public class LocationLocalRepository : ILocationRepository
//	{
//        private ApplicationDbContext context;
//        public IEnumerable<LocationModel> Locations => context.Locations;
//		public LocationLocalRepository(ApplicationDbContext context)
//        {
//            this.context = context;
//		}
//		public LocationModel GetLocationByID(string LocationId)
//        {
//            return context.Locations.Find(LocationId);
//		}
//        public void AddLocation(LocationModel Location)
//        {
//            context.Locations.Add(Location);
//		}
//        public void RemoveLocation(LocationModel Location)
//        {
//            context.Locations.Remove(Location);
//		}
//		public void RemoveLocationByID(string LocationId)
//        {
//            context.Locations.Remove(GetLocationByID(LocationId));
//		}
//		public void UpdateLocation(LocationModel Location)
//        {
//            context.Locations.Update(Location);
//		}
//        public void Save()
//        {
//            context.SaveChanges();
//		}
//    }
//}
