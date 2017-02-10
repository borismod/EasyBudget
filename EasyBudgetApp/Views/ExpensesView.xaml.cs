using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp.Views
{
    /// <summary>
    /// Interaction logic for ExpensesView.xaml
    /// </summary>
    public partial class ExpensesView : UserControl
    {
        public ExpensesView()
        {
            InitializeComponent();
        }

        private void OnCategoryChanged(object sender, TreeListCellValueChangedEventArgs e)
        {
            if (!e.Column.FieldName.Equals("Category") || e.OldValue == e.Value)
            {
                return;
            }

            var expenses = DataContext as ExpensesViewModel;

            var currentTransaction = expenses?.TransactionsView.CurrentItem as TransactionViewModel;
            if (currentTransaction == null)
            {
                return;
            }

            expenses.CategoriesMap.AddSupplierToCategory(currentTransaction.SupplierId, (Guid)e.Value);
        }
    }
}
