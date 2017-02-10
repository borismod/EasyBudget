using System;
using System.Threading.Tasks;
using EasyBudgetApp.Views;
using MahApps.Metro.Controls.Dialogs;

namespace EasyBudgetApp
{
    public class MessageService
    {
        private MainView _view;

        public MessageService(MainView view)
        {
            _view = view;
        }

        public async Task<MessageDialogResult> ShowMessage(string message)
        {
            return await _view.ShowMessageAsync("Message", message);
        }
    }
}
