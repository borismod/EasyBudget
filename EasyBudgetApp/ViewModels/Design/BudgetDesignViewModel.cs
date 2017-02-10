using System;
using System.Collections.ObjectModel;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels.Design
{
    public class BudgetDesignViewModel
    {
        public ObservableCollection<CategoryViewModel> Categories { get; set; }

        public BudgetDesignViewModel()
        {
            Categories = new ObservableCollection<CategoryViewModel>();
            Categories.Add(new CategoryViewModel(new Category(Guid.NewGuid(), "Home")));
            Categories.Add(new CategoryViewModel(new Category(Guid.NewGuid(), "Services")));
            Categories.Add(new CategoryViewModel(new Category(Guid.NewGuid(), "Meal")));
            Categories.Add(new CategoryViewModel(new Category(Guid.NewGuid(), "Pharm")));
        }
    }
}
