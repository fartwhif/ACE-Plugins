using ACE.Common;
using ACE.Plugin.Web.Common;
using ACE.Plugin.Web.Managers;
using log4net;
using System.Reflection;

namespace ACE.Plugin.Web
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
            WebGlobal.ResultOfInitSink = ResultOfInitSink;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            WebConfigManager.Initialize();
            log.Info("Pre-Initializing WebManager...");
            WebManager.PreInitialize();
            ResultOfInitSink.SetResult(true); //report successful pre initialization
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            WebManager.StopWebServices();
        }

        public void AllPluginsStarted(TaskCompletionSource<bool> AllPluginsStartedSink)
        {
            // other plugins have had time to register their nancy modules
            // start the host now
            log.Info("Initializing WebManager...");
            WebGlobal.ResultOfHostRunSink = AllPluginsStartedSink;
            WebManager.StartWebServices();
        }
    }
}
