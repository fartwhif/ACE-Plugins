using ACE.Common;
using ACE.Plugin.Crypto.Common;
using System.Threading.Tasks;

namespace ACE.Plugin.Crypto.Managers
{
    public class CryptoInitializer : IACEPlugin
    {
        public void Start(TaskCompletionSource<bool> tsc)
        {
            PluginConstants.MyInstance = this;
            CryptoConfigManager.Initialize();
            CertificateManager.Initialize();
            tsc.SetResult(true); // report success
        }
    }
}