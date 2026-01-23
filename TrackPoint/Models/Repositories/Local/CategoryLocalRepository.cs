//using TrackPoint.Data;
//using TrackPoint.Models.RepositoryInterfaces;

//namespace TrackPoint.Models.Repositories.Local
//{
//	/**
//     * This repository interacts with the local database to manage Categories
//     */
//	public class CategoryLocalRepository : ICategoryRepository
//	{
//        private ApplicationDbContext context;
//        public IEnumerable<CategoryModel> Categories => context.Categories;
//		public CategoryLocalRepository(ApplicationDbContext context)
//        {
//            this.context = context;
//		}
//		public CategoryModel GetCategoryByID(string CategoryId)
//        {
//            return context.Categories.Find(CategoryId);
//		}
//        public void AddCategory(CategoryModel Category)
//        {
//            context.Categories.Add(Category);
//		}
//        public void RemoveCategory(CategoryModel Category)
//        {
//            context.Categories.Remove(Category);
//		}
//		public void RemoveCategoryByID(string CategoryId)
//        {
//            context.Categories.Remove(GetCategoryByID(CategoryId));
//		}
//		public void UpdateCategory(CategoryModel Category)
//        {
//            context.Categories.Update(Category);
//		}
//        public void Save()
//        {
//            context.SaveChanges();
//		}
//    }
//}
