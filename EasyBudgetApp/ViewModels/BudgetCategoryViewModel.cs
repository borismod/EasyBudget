using System;
using System.Windows.Media;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
    public class BudgetCategoryViewModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        private double _planned;
        public double Planned
        {
            get { return _planned; }
            set
            {
                _planned = value;
                OnPropertyChanged(() => Planned);
            }
        }

        private double _spent;
        public double Spent
        {
            get { return _spent;}
            set
            {
                _spent = value;
                OnPropertyChanged(()=>Spent);
            }
        }
        public Brush Color { get; set; }

        public double Percentage => (Spent/Planned)*100;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        public BudgetCategoryViewModel(Category category)
        {
            Name = category.Name;
            Id = category.Id;
            Spent = 0;
        }

        public BudgetCategoryViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
