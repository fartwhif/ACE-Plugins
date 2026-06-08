using ACE.Server.Plugin;

namespace ACE.Plugin.Warp.Common
{
    public static class WarpConfigManager
    {
        public static WarpConfiguration Config => Log4netEnabledPluginConfigManager<WarpConfigurationOuter>.Config.WarpConfiguration;
        public static void Initialize()
        {
            Log4netEnabledPluginConfigManager<WarpConfigurationOuter>.Initialize();
        }
    }
}
