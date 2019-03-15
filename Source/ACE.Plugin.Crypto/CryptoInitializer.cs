using ACE.Common;
using ACE.Plugin.Crypto.Common;
using ACE.Plugin.Crypto.Managers;
using System.Threading.Tasks;

namespace ACE.Plugin.Crypto
{
    public class CryptoInitializer : IACEPlugin
    {
        public void Start(TaskCompletionSource<bool> ResultOfInitSink)
        {
            PluginConstants.MyInstance = this;
            CryptoConfigManager.Initialize();
            CertificateManager.Initialize();
            ResultOfInitSink.SetResult(true); // report success
        }
    }
}