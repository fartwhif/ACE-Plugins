using ACE.Plugin.WebAPI.Model;
using Nancy;

namespace ACE.Plugin.WebAPI.Modules
{
    public class BaseAuthenticatedModule : NancyModule
    {
        public BaseAuthenticatedModel BaseModel
        {
            get
            {
                BaseAuthenticatedModel model = new BaseAuthenticatedModel
                {
                    AccessLevelName = Context.CurrentUser.FindFirst("AccessLevelName").Value,
                    AccountName = Context.CurrentUser.Identity.Name
                };
                return model;
            }
        }
    }
}
