using ACE.Server.Plugin;

namespace ACE.Plugin.Web.Common
{
    public static class WebConfigManager
    {
        public static WebConfiguration Config => Log4netEnabledPluginConfigManager<WebConfigurationOuter>.Config.WebConfiguration;
        public static void Initialize()
        {
            Log4netEnabledPluginConfigManager<WebConfigurationOuter>.Initialize();
        }
    }
}
