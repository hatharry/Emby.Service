using System.Diagnostics;
using System.ServiceProcess;

namespace Emby.Service
{
    public partial class EmbyService : ServiceBase
    {
        public static string Name = "Emby";
        public static string DisplayName = "Emby Server";
        public static string appName = "MediaBrowser.ServerApplication";

        public static string GetExistingServiceName()
        {
            return Name;
        }
        
        public EmbyService()
        {
            InitializeComponent();
        }

        protected override void OnStop()
        {
            foreach (var process in Process.GetProcessesByName(appName))
            {
                process.Kill();
            }
            base.OnStop();
        }
    }
}
