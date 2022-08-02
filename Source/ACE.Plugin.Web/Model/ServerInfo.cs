using ACE.Common;

namespace ACE.Plugin.Web.Model
{
    public class ServerInfoResponseModel
    {
        public string WorldName { get; set; }
        public string ServerMotd { get; set; }
        public string PopupMotd { get; set; }
        public double Uptime { get; set; }
        public PlayerCountResponseModel PlayerCount { get; set; }
        public AccountDefaults AccountDefaults { get; set; }
        public uint MaximumAllowedSessions { get; set; }
    }
}
