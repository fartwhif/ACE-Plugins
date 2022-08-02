using Microsoft.Extensions.DependencyInjection;

namespace ACE.Plugin.Web
{
    public class StartupBase
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }
    }
}