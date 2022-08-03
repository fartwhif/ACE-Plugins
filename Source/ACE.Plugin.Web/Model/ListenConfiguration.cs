using System.Net;

namespace ACE.Plugin.Web.Model
{
    internal class ListenConfiguration
    {
        public IPAddress Address { get; set; }
        public ushort Port { get; set; }
    }
}
