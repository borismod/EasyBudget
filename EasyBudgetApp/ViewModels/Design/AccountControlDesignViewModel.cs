using System.Diagnostics.PerformanceData;
using System.Windows;

namespace EasyBudgetApp.ViewModels.Design
{
    public class AccountControlDesignViewModel
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Details { get; set; }
        public double Pereodic{ get; set; }
        public double Total { get; set; }
        public string Error { get; set; }
        public string ErrorMessage { get; set; }
        public RelayCommand RemoveAccount { get; set; }
        public AccountControlDesignViewModel()
        {
            CompanyName = "Bank Hapoalim";
            Name = "232-432234";
            Id = 4883;
            Details = "This is a demo account";
            //Error = "Error";
            //ErrorMessage = "Cannot Access";
            Pereodic = 123.40;
            Total = 2354.30;
            //Total = double.NaN;
        }
    }
}
