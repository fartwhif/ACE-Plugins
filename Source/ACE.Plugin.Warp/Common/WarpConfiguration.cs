using System.Collections.Generic;

namespace ACE.Plugin.Warp.Common
{
    public class WarpConfigurationOuter
    {
        public WarpConfiguration WarpConfiguration { get; set; }
    }

    public class WarpConfiguration
    {
        /// <summary>
        /// Messages randomly shown to players when they collide with the warp portal.
        /// </summary>
        public List<string> CollisionMessages { get; set; } = new List<string>
        {
            "You have entered the Rithwic portal zone."
        };

        /// <summary>
        /// Display name of the destination server (shown to player).
        /// </summary>
        public string DestinationServer { get; set; } = "";

        /// <summary>
        /// Internal URL of the destination server's Web API (for migration requests).
        /// </summary>
        public string DestinationServerUrl { get; set; } = "";

        /// <summary>
        /// Host address of the destination server's game protocol (for client reconnection).
        /// </summary>
        public string DestinationServerHost { get; set; } = "127.0.0.1";

        /// <summary>
        /// UDP port of the destination server's game protocol.
        /// </summary>
        public int DestinationServerPort { get; set; } = 9004;
    }
}
