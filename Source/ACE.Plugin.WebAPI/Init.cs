using ACE.Common;
using ACE.Plugin.WebAPI.Common;
using ACE.Plugin.WebAPI.Managers;
using log4net;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ACE.Plugin.WebAPI
{
    public class Init : IACEPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Start(TaskCompletionSource<bool> ResultOfInitSink)
        {
            if (!Assembly.GetCallingAssembly().GetName().FullName.StartsWith("ACE.Server,"))
            {
                log.Fatal("Invalid startup method.  This is an ACEmulator plugin.");
                return;
            }
            WebAPIGlobal.ResultOfInitSink = ResultOfInitSink;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            WebAPIConfigManager.Initialize();
            log.Info("Pre-Initializing WebManager...");
            WebManager.PreInitialize();
            ResultOfInitSink.SetResult(true); //report successful pre initialization
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            WebManager.Shutdown();
        }

        public void AllPluginsStarted(TaskCompletionSource<bool> AllPluginsStartedSink)
        {
            // other plugins have had time to register their nancy modules
            // start the host now
            log.Info("Initializing WebManager...");
            WebAPIGlobal.ResultOfHostRunSink = AllPluginsStartedSink;
            WebManager.Initialize();
        }
    }
}
