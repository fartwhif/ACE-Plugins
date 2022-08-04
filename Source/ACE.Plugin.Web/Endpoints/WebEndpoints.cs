using log4net;
using System.Reflection;

namespace ACE.Plugin.Web
{
    static partial class WebEndpoints
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
