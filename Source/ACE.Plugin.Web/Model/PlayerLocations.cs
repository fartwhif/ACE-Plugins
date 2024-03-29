using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Plugin.Web.Model
{
    public class PlayerNameAndLocation
    {
        public string Location { get; set; }
        public string Name { get; set; }
    }
    public class PlayerLocationsResponseModel
    {
        public List<PlayerNameAndLocation> Locations { get; set; }
    }
}
