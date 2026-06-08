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
    }
}
