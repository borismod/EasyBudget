using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace EasyBudgetApp
{
    public class Installer
    {
        private const ShortcutLocation DefaultLocations = ShortcutLocation.StartMenu|ShortcutLocation.Desktop;
        private readonly MessageService _messageService;

        public Installer(MessageService messageService)
        {
            _messageService = messageService;
            var updateTask = new TaskFactory().StartNew(UpdateApp).Result;
        }

        private async Task UpdateApp()
        {
            var updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            using (var updateManager = new UpdateManager(updatePath, applicationName))
            {
                try
                {
                    var updates = await updateManager.CheckForUpdate();
                    if (updates.ReleasesToApply.Any())
                    {
                        var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();
                        await updateManager.DownloadReleases(new[] { lastVersion });
                        await updateManager.ApplyReleases(updates);
                        await updateManager.UpdateApp();

                        await _messageService.ShowMessage("EasyBudget application has been updated by newer version.\n Please press OK to restart.");
                        
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        public static void OnAppUpdate(Version version)
        {
            // Could use this to do stuff here too.
        }

        public static void OnInitialInstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            var updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            using (var updateManager = new UpdateManager(updatePath, applicationName))
            {
                // Create Desktop and Start Menu shortcuts
                updateManager.CreateShortcutsForExecutable(appName, DefaultLocations, false);
            }
        }

        public static void OnAppUninstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            var updatePath = ConfigurationManager.AppSettings["UpdatePathFolder"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            using (var updateManager = new UpdateManager(updatePath, applicationName))
            {
                // Remove Desktop and Start Menu shortcuts
                updateManager.RemoveShortcutsForExecutable(appName, DefaultLocations);
            }
        }
    }
}
