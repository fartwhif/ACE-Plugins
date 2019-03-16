using ACE.Server.Plugin;

namespace ACE.Plugin.WebAPI.Common
{
    public static class WebAPIConfigManager
    {
        public static WebAPIConfiguration Config => Log4netEnabledPluginConfigManager<WebAPIConfigurationOuter>.Config.WebAPIConfiguration;
        public static void Initialize()
        {
            Log4netEnabledPluginConfigManager<WebAPIConfigurationOuter>.Initialize();
        }
    }
}
