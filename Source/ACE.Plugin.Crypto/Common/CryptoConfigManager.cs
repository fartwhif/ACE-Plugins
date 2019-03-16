using ACE.Server.Plugin;

namespace ACE.Plugin.Crypto.Common
{
    public static class CryptoConfigManager
    {
        public static CryptoConfiguration Config => Log4netEnabledPluginConfigManager<CryptoConfigurationOuter>.Config.CryptoConfiguration;
        public static void Initialize()
        {
            Log4netEnabledPluginConfigManager<CryptoConfigurationOuter>.Initialize();
        }
    }
}
