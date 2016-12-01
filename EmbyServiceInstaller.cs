using System.Collections;
using System.ComponentModel;
using System.ServiceProcess;

namespace Emby.Service
{
    [RunInstaller(true)]
    public class EmbyServiceInstaller : System.Configuration.Install.Installer
    {
        public EmbyServiceInstaller()
        {
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            var serviceAdmin = new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = EmbyService.Name,
                DisplayName = EmbyService.DisplayName,

                DelayedAutoStart = true,

                Description = "The windows background service for Emby Server.",

                // Will ensure the network is available
                ServicesDependedOn = new[] { "LanmanServer", "EventLog", "Tcpip", "http" }
            };

            // Microsoft didn't add the ability to add a
            // description for the services we are going to install
            // To work around this we'll have to add the
            // information directly to the registry but I'll leave
            // this exercise for later.

            // now just add the installers that we created to our
            // parents container, the documentation
            // states that there is not any order that you need to
            // worry about here but I'll still
            // go ahead and add them in the order that makes sense.
            Installers.Add(process);
            Installers.Add(serviceAdmin);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            Context.Parameters["assemblypath"] = "\"" +
                Context.Parameters["assemblypath"] + "\" " + GetStartArgs();
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            Context.Parameters["assemblypath"] = "\"" +
                Context.Parameters["assemblypath"] + "\" " + GetStartArgs();
            base.OnBeforeUninstall(savedState);
        }

        private string GetStartArgs()
        {
            return "-nosplash -notray -norestart";
        }
    }
}
