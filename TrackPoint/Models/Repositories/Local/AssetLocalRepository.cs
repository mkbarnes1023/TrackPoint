using TrackPoint.Data;
using TrackPoint.Models.RepositoryInterfaces;

namespace TrackPoint.Models.Repositories.Local
{
	/**
     * This repository interacts with the local database to manage Assets
     */
	public class AssetLocalRepository : IAssetRepository
	{
        private ApplicationDbContext context;
        public IEnumerable<AssetModel> Assets => context.Assets;
		public AssetLocalRepository(ApplicationDbContext context)
        {
            this.context = context;
		}
		public AssetModel GetAssetByID(string assetId)
        {
            return context.Assets.Find(assetId);
		}
        public void AddAsset(AssetModel asset)
        {
            context.Assets.Add(asset);
		}
        public void RemoveAsset(AssetModel asset)
        {
            context.Assets.Remove(asset);
		}
		public void RemoveAssetByID(string assetId)
        {
            context.Assets.Remove(GetAssetByID(assetId));
		}
		public void UpdateAsset(AssetModel asset)
        {
            context.Assets.Update(asset);
		}
        public void Save()
        {
            context.SaveChanges();
		}
    }
}
