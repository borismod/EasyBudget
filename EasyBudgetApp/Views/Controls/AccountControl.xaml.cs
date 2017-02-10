using System.Windows;
using System.Windows.Controls;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp.Views.Controls
{
    /// <summary>
    /// Interaction logic for AccountControl.xaml
    /// </summary>
    public partial class AccountControl : UserControl
    {
        public static readonly DependencyProperty IsSwitchVisibleProperty =
          DependencyProperty.Register("IsSwitchVisible", typeof(bool), typeof(AccountControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsPeriodicVisibleProperty =
          DependencyProperty.Register("IsPeriodicVisible", typeof(bool), typeof(AccountControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsTotalVisibleProperty =
          DependencyProperty.Register("IsTotalVisible", typeof(bool), typeof(AccountControl), new PropertyMetadata(true));

        public static readonly DependencyProperty RemoveAccountCommandProperty =
          DependencyProperty.Register("RemoveAccountCommand", typeof(RelayCommand), typeof(AccountControl), new PropertyMetadata(null));

        public bool IsSwitchVisible
        {
            get
            {
                return (bool)GetValue(IsSwitchVisibleProperty);
            }
            set
            {
                SetValue(IsSwitchVisibleProperty, value);
            }
        }

        public bool IsPeriodicVisible
        {
            get
            {
                return (bool)GetValue(IsPeriodicVisibleProperty);
            }
            set
            {
                SetValue(IsPeriodicVisibleProperty, value);
            }
        }

        public bool IsTotalVisible
        {
            get
            {
                return (bool)GetValue(IsTotalVisibleProperty);
            }
            set
            {
                SetValue(IsTotalVisibleProperty, value);
            }
        }
       
        public RelayCommand RemoveAccountCommand
        {
            get
            {
                return (RelayCommand)GetValue(RemoveAccountCommandProperty);
            }
            set
            {
                SetValue(RemoveAccountCommandProperty, value);
            }
        }
        
        public AccountControl()
        {
            InitializeComponent();
        }
    }
}
