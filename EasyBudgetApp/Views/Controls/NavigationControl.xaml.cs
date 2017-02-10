using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp.Views.Controls
{
    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(NavigationControl), new PropertyMetadata("Title"));

        public static readonly DependencyProperty BackButtonProperty =
          DependencyProperty.Register("BackButtonCommand", typeof(RelayCommand), typeof(NavigationControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ForwardButtonProperty =
          DependencyProperty.Register("ForwardButtonCommand", typeof(RelayCommand), typeof(NavigationControl), new PropertyMetadata(null));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public RelayCommand BackButtonCommand
        {
            get
            {
                return (RelayCommand)GetValue(BackButtonProperty);
            }
            set
            {
                SetValue(BackButtonProperty, value);
            }
        }

        public RelayCommand ForwardButtonCommand
        {
            get
            {
                return (RelayCommand)GetValue(ForwardButtonProperty);
            }
            set
            {
                SetValue(ForwardButtonProperty, value);
            }
        }

        public NavigationControl()
        {
            InitializeComponent();
        }
    }
}
