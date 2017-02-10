using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using EasyBudgetApp.Models.Credentials;
using EasyBudgetApp.ViewModels;
using EasyBudgetApp.Views;
using Telemetry;

namespace EasyBudgetApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Installer _appInstaller;

        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            base.OnStartup(eventArgs);

            TelemetryService.Initialize();
            TelemetryService.PublishUsage(Environment.MachineName, GetAppAssemblyVersion(), "OnStartup", Phase.Start);

            log4net.Config.XmlConfigurator.Configure();

            Uri uri = new Uri("_Resources/Icons.xaml", UriKind.Relative);
            ResourceDictionary dict = new ResourceDictionary { Source = uri };
            
            var mainView = new MainView();
            var messageService = new MessageService(mainView);
            _appInstaller = new Installer(messageService);

            var toolbar = new ToolbarViewModel();
            var profileManager = new UserProfileManager();
            var activityManager = new InactiveTransactionsManager();
            var categoriesMap = new CategoriesViewModel();
            var dataLoader = new DataLoader(profileManager);

            var dateStateVm = new DateStateViewModel();
            var auditor = new Auditor(dataLoader, dateStateVm);
            var newAccountVm = new NewAccountViewModel(dataLoader);
            var newCategoryVm = new NewCategoryViewModel();

            var accountsVm = new AccountsViewModel(dataLoader, auditor, dateStateVm);
            var incomeVm = new IncomeViewModel(dataLoader, auditor, dateStateVm);
            var expensesVm = new ExpensesViewModel(dataLoader, auditor, dateStateVm, categoriesMap);
            var budgetVm = new BudgetViewModel(dataLoader, auditor, dateStateVm, categoriesMap);

            var analysisVm = new AnalysisViewModel();
            var settingsVm = new SettingsViewModel();
            var mainVm = new MainViewModel(dataLoader, toolbar, accountsVm, newAccountVm, newCategoryVm, budgetVm, dateStateVm);

            toolbar.Items.Add(new ToolboxItemViewModel("Accounts", dict["appbar_layer"] as Canvas, accountsVm, mainVm, true));
            toolbar.Items.Add(new ToolboxItemViewModel("Income", dict["appbar_graph_line_up"] as Canvas, incomeVm, mainVm));
            toolbar.Items.Add(new ToolboxItemViewModel("Expenses", dict["appbar_graph_line_down"] as Canvas, expensesVm, mainVm));
            toolbar.Items.Add(new ToolboxItemViewModel("Budget", dict["appbar_billing"] as Canvas, budgetVm, mainVm));
            toolbar.Items.Add(new ToolboxItemViewModel("Analysis", dict["appbar_graph_bar"] as Canvas, analysisVm, mainVm));
            toolbar.Items.Add(new ToolboxItemViewModel("Settings", dict["appbar_settings"] as Canvas, settingsVm, mainVm));

            mainView.DataContext = mainVm;
            mainView.Show();
            mainView.Closing += (s, e) =>
            {
                mainVm.Dispose();
                accountsVm.Dispose();
                expensesVm.Dispose();
                activityManager.Dispose();

                TelemetryService.PublishUsage(Environment.MachineName, GetAppAssemblyVersion(), "Closing", Phase.End);
            };
            
            dataLoader.LoadAsync();

            TelemetryService.PublishUsage(Environment.MachineName, GetAppAssemblyVersion(), "OnStartup", Phase.End);
        }


        private string GetAppAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}
