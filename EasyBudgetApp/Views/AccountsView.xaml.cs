using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Xpf.Grid.TreeList;

namespace EasyBudgetApp.Views
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        private void OnCellValueChanging(object sender, TreeListCellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.Equals("IsActive"))
            {
                listView.CommitEditing();
            }
        }
    }
}
