using System;
using System.Collections.Generic;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public String Name { get; private set; }
        public Guid Id { get; private set; }
        public List<long> Suppliers { get; private set; }
        public double Amount { get; set; }

        public CategoryViewModel(Category category)
        {
            Name = category.Name;
            Id = category.Id;
            Suppliers = category.Suppliers;
            Amount = 0;
        }
    }
}
