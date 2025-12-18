using TrackPoint.Models.RepositoryInterfaces;

namespace TrackPoint.Models.Repositories.Database
{
	/**
	 * This repository interacts with the database to manage Categories
	 */
    public class CategoryDatabaseRepository : ICategoryRepository
	{
		public IEnumerable<CategoryModel> Categories { get; }
		public CategoryModel GetCategoryByID(string categoryId)
		{
			throw new NotImplementedException();
		}
		public void AddCategory(CategoryModel category)
		{
			throw new NotImplementedException();
		}
		public void RemoveCategory(CategoryModel category)
		{
			throw new NotImplementedException();
		}
		public void RemoveCategoryByID(string categoryId)
		{
			throw new NotImplementedException();
		}
		public void UpdateCategory(CategoryModel categoryId)
		{
			throw new NotImplementedException();
		}
		public void Save()
		{
			throw new NotImplementedException();
		}
	}
}
