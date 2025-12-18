using TrackPoint.Models.RepositoryInterfaces;

namespace TrackPoint.Models.Repositories.Database
{
	/**
     * This repository interacts with a database to manage Assets
     */
	public class AssetDatabaseRepository : IAssetRepository
	{
        public IEnumerable<AssetModel> Assets { get; }
		public AssetModel GetAssetByID(string assetId)
        {
            throw new NotImplementedException();
		}
        public void AddAsset(AssetModel asset)
        {
            throw new NotImplementedException();
		}
		public void RemoveAsset(AssetModel asset)
        {
            throw new NotImplementedException();
		}
		public void RemoveAssetByID(string assetId)
        {
            throw new NotImplementedException();
		}
		public void UpdateAsset(AssetModel asset)
        {
            throw new NotImplementedException();
		}
		public void Save()
        {
            throw new NotImplementedException();
		}
    }
}
