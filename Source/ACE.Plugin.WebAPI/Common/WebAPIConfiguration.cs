namespace ACE.Plugin.WebAPI.Common
{
    public class WebAPIConfigurationOuter
    {
        public WebAPIConfiguration WebAPIConfiguration { get; set; }
    }
    public class WebAPIConfiguration
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
