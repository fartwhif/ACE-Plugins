using System.Net;

namespace ACE.Plugin.WebAPI.Model
{
    internal class Perch
    {
        public IPAddress Address { get; set; }
        public ushort Port { get; set; }
    }
}
