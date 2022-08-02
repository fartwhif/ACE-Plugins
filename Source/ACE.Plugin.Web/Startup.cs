using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ACE.Plugin.Web.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ACE.Plugin.Web.Managers;
using Microsoft.Extensions.FileProviders;

namespace ACE.Plugin.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var asm = Assembly.GetAssembly(typeof(Startup));
        var loc = asm.Location;
        var pluginPath = Path.GetDirectoryName(loc);
        var path_wwwroot = Path.Combine(pluginPath, "wwwroot");
        env.ContentRootPath = pluginPath;
        env.WebRootPath = path_wwwroot;

        WebLogging.ConfigureLogger(loggerFactory);
        WebLogging.LoggerFactory = loggerFactory;

        if (env.EnvironmentName == EnvironmentName.Development)
        {
            app.UseDeveloperExceptionPage();
        }

        PhysicalFileProvider fileProvider = new PhysicalFileProvider(path_wwwroot);

        DefaultFilesOptions options = new DefaultFilesOptions
        {
            FileProvider = fileProvider,
            RequestPath = "",
        };
        options.DefaultFileNames.Add("Default.htm");
        app.UseDefaultFiles(options);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider,
            RequestPath = ""
        });

    }
}