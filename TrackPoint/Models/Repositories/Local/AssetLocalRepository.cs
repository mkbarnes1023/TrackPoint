//using TrackPoint.Data;
//using TrackPoint.Models.RepositoryInterfaces;

//namespace TrackPoint.Models.Repositories.Local
//{
//	/**
//     * This repository interacts with the local database to manage Assets
//     */
//	public class AssetLocalRepository : IAssetRepository
//	{
//        private ApplicationDbContext context;
//        public IEnumerable<Asset> Assets => context.Asset;
//		public AssetLocalRepository(ApplicationDbContext context)
//        {
//            this.context = context;
//		}
//		public Asset GetAssetByID(string assetId)
//        {
//            return context.Asset.Find(assetId);
//		}
//        public void AddAsset(Asset asset)
//        {
//            context.Asset.Add(asset);
//		}
//        public void RemoveAsset(Asset asset)
//        {
//            context.Asset.Remove(asset);
//		}
//		public void RemoveAssetByID(string assetId)
//        {
//            context.Asset.Remove(GetAssetByID(assetId));
//		}
//		public void UpdateAsset(Asset asset)
//        {
//            context.Asset.Update(asset);
//		}
//        public void Save()
//        {
//            context.SaveChanges();
//		}
//    }
//}
