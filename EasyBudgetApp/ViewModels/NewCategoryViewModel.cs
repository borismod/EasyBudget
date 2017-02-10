using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace EasyBudgetApp.ViewModels
{
    public class NewCategoryViewModel : BaseViewModel
    {
        private static event EventHandler<EventArgs> NewCategoryAdded;
        public static IObservable<EventArgs> WhenNewCategoryAdded
        {
            get
            {
                return Observable.FromEventPattern<EventArgs>(
                        h => NewCategoryAdded += h,
                        h => NewCategoryAdded -= h)
                    .Select(x => x.EventArgs);
            }
        }

        public string Name { get; set; }

        public ObservableCollection<PropertyInfo> _categoryColors;
        public ObservableCollection<PropertyInfo> CategoryColors
        {
            get { return _categoryColors; }
            set
            {
                _categoryColors = value;
                OnPropertyChanged(() => CategoryColors);
            }
        }

        private PropertyInfo _selectedColor;
        public PropertyInfo SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyChanged(() => SelectedColor);
            }
        }


        public BudgetCategoryViewModel NewCategory { get; set; }

        public NewCategoryViewModel()
        {
            GenerateBrushes();
            
            NewCategory = new BudgetCategoryViewModel(Guid.NewGuid(), "");
        }

        private void GenerateBrushes()
        {
            CategoryColors = new ObservableCollection<PropertyInfo>();

            var properties = typeof(Colors).GetProperties();
            foreach (var color in properties)
            {
                CategoryColors.Add(color);
            }
        }
    }
}
