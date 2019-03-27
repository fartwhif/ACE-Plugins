using ACE.Server.Plugin;

namespace ACE.Plugin.Transfer.Common
{
    public static class TransferConfigManager
    {
        public static TransferConfiguration Config => Log4netEnabledPluginConfigManager<TransferConfigurationOuter>.Config.TransferConfiguration;
        public static void Initialize()
        {
            Log4netEnabledPluginConfigManager<TransferConfigurationOuter>.Initialize();
        }
    }
}
