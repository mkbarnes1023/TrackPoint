namespace TrackPoint.Models.RepositoryInterfaces
{
    public interface IAssetRepository
    {
        IEnumerable<AssetModel> GetAssets();
		AssetModel GetAssetByID(string assetId);
        void AddAsset(AssetModel asset);
        void RemoveAsset(AssetModel asset);
		void RemoveAssetByID(string assetId);
		void UpdateAsset(AssetModel asset);
        void Save();
    }
}
