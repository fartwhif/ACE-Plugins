namespace ACE.Plugin.Web.Common
{
    public class WebConfigurationOuter
    {
        public WebConfiguration WebConfiguration { get; set; }
    }
    public class WebConfiguration
    {
        public string Host { get; set; }
        public ushort Port { get; set; }

        /// <summary>
        /// Used to form WAN accessible URIs
        /// </summary>
        public string ExternalIPAddressOrDNSName { get; set; }

        /// <summary>
        /// Used to form WAN accessible URIs
        /// </summary>
        public string ExternalPort { get; set; }
    }
}
