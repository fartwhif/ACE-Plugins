using ACE.Common;
using ACE.Plugin.Crypto.Common;
using ACE.Plugin.Crypto.Managers;
using log4net;
using System.Reflection;
using System.Threading.Tasks;

namespace ACE.Plugin.Crypto
{
    public class Init : IACEPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void AllPluginsStarted(TaskCompletionSource<bool> AllPluginsStartedSink)
        {
            AllPluginsStartedSink.SetResult(true);
        }
        public void Start(TaskCompletionSource<bool> ResultOfInitSink)
        {
            if (!Assembly.GetCallingAssembly().GetName().FullName.StartsWith("ACE.Server,"))
            {
                log.Fatal("Invalid startup method.  This is an ACEmulator plugin.");
                return;
            }
            CryptoConfigManager.Initialize();
            CertificateManager.Initialize();
            ResultOfInitSink.SetResult(true); // report success
        }
    }
}