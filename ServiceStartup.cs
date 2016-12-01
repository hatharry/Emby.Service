using MediaBrowser.Server.Startup.Common;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Emby.Service
{
    static class ServiceStartup
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var options = new StartupOptions();
            var currentProcess = Process.GetCurrentProcess();
            var applicationName = "MediaBrowser.ServerApplication.exe";
            var servicePath = currentProcess.MainModule.FileName;
            var applicationPath = Path.Combine(Path.GetDirectoryName(servicePath), applicationName);

            if (options.ContainsOption("-installservice"))
            {
                InstallService(servicePath);
                return;
            }

            if (options.ContainsOption("-uninstallservice"))
            {
                UninstallService(servicePath);
                return;
            }
            Task.Factory.StartNew(() => Run(applicationPath));
            RunService();
        }
        private static void RunService() { 
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EmbyService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void Run(string applicationPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = applicationPath,

                Arguments = "-nosplash -notray -norestart",

                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas",
                ErrorDialog = false
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 3)
                {
                    Run(applicationPath);
                }
            }

            ShutdownWindowsService();

        }

        private static void ShutdownWindowsService()
        {
            var service = new ServiceController(EmbyService.GetExistingServiceName());

            service.Refresh();

            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
            }
        }

        private static void InstallService(string applicationPath)
        {
            ManagedInstallerClass.InstallHelper(new[] { applicationPath });
        }

        private static void UninstallService(string applicationPath)
        {
            ManagedInstallerClass.InstallHelper(new[] { "/u", applicationPath });
        }
    }
}
