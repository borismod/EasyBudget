using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using EasyBudgetApp.Models;
using Newtonsoft.Json;

namespace EasyBudgetApp.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        private readonly string _categoriesMapFilePath;

        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        
        // category ID to category
        public IDictionary<Guid, CategoryViewModel> CategoriesById { get; private set; }

        // suppliers to category ID
        public static IDictionary<long, Guid> Suppliers { get; set; }

        private static event EventHandler<CategoryEventArgs> CategoryUpdated;
        public static IObservable<CategoryEventArgs> WhenCategoryUpdated
        {
            get
            {
                return Observable.FromEventPattern<CategoryEventArgs>(
                        h => CategoryUpdated += h,
                        h => CategoryUpdated -= h)
                    .Select(x => x.EventArgs);
            }
        }

        public CategoriesViewModel()
        {
            _categoriesMapFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Categories.json");
            PopulateCategories();
        }

        public void AddSupplierToCategory(long supplier, Guid category)
        {
            if (Suppliers.ContainsKey(supplier))
            {
                var categoryId = Suppliers[supplier];
                if (CategoriesById.ContainsKey(categoryId))
                {
                    var c = GetCategoryByCategoryId(categoryId);
                    c.Suppliers.Remove(supplier);
                }

                Suppliers[supplier] = category;
            }
            else
            {
                Suppliers.Add(supplier, category);
            }

            if (CategoriesById.ContainsKey(category))
            {
                GetCategoryByCategoryId(category).Suppliers.Add(supplier);
            }

            OnCategoryUpdated(GetCategoryByCategoryId(category), supplier);
        }

        private CategoryViewModel GetCategoryByCategoryId(Guid categoryId)
        {
            var result = Categories.FirstOrDefault(category => category.Id.Equals(categoryId));
            return result;
        }

        private void PopulateCategories()
        {
            var categories = LoadCategories();

            CategoriesById = new Dictionary<Guid, CategoryViewModel>();
            Categories = new ObservableCollection<CategoryViewModel>();
            Suppliers = new Dictionary<long, Guid>();

            foreach (var category in categories)
            {
                var categoryVm = new CategoryViewModel(category);
                Categories.Add(categoryVm);
                CategoriesById.Add(category.Id, categoryVm);

                foreach (var supplier in category.Suppliers)
                {
                    if (!Suppliers.ContainsKey(supplier))
                    {
                        Suppliers.Add(supplier, category.Id);
                    }
                }
            }
        }

        private IList<Category> LoadCategories()
        {
            if (string.IsNullOrEmpty(_categoriesMapFilePath) || !File.Exists(_categoriesMapFilePath))
            {
                throw new ArgumentException(@"Incorrect value of categories - {0}", _categoriesMapFilePath);
            }

            IList<Category> categories;
            using (StreamReader file = File.OpenText(_categoriesMapFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                categories = (List<Category>)serializer.Deserialize(file, typeof(List<Category>));
            }

            return categories ?? new List<Category>();
        }

        private void SaveCategories()
        {
            using (StreamWriter file = File.CreateText(_categoriesMapFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, Categories);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            SaveCategories();
        }

        private void OnCategoryUpdated(CategoryViewModel category, long supplier)
        {
            EventHandler<CategoryEventArgs> eventHandler = CategoryUpdated;
            eventHandler?.Invoke(this, new CategoryEventArgs(category, supplier));
        }

        public static Guid GetCategoryBySupplier(long supplierId)
        {
            if (Suppliers.ContainsKey(supplierId))
            {
                return Suppliers[supplierId];
            }

            return Guid.Empty;
        }
    }

    public class CategoryEventArgs
    {
        internal CategoryViewModel Category { get; }
        internal long Supplier { get; }
        internal CategoryEventArgs(CategoryViewModel category, long supplier)
        {
            Category = category;
            Supplier = supplier;
        }
    }
}
