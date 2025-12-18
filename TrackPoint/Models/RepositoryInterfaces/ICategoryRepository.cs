namespace TrackPoint.Models.RepositoryInterfaces
{
    public interface ICategoryRepository
    {
		IEnumerable<CategoryModel> Categories { get; }
		CategoryModel GetCategoryByID(string categoryId);
		void AddCategory(CategoryModel category);
		void RemoveCategory(CategoryModel category);
		void RemoveCategoryByID(string categoryId);
		void UpdateCategory(CategoryModel categoryId);
		void Save();
	}
}
