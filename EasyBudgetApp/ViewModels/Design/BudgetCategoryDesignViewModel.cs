using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace EasyBudgetApp.ViewModels.Design
{
    public class BudgetCategoryDesignViewModel
    {
        public String Name { get; set; }
        public int Id { get; set; }
        public List<long> Suppliers { get; set; }
        public double Planned { get; set; }
        public double Spent { get; set; }
        public double Percentage { get; set; }
        public Brush Color { get; set; }
        public BudgetCategoryDesignViewModel()
        {
            Name = "Home";
            Id = 1334;
            Planned = 4353.00;
            Spent = 3345.00;
            Color = Brushes.Gold;

            Percentage = (Spent/Planned)*100;
        }
    }
}
