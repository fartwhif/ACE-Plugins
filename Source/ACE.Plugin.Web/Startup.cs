using ACE.Plugin.Web.Logging;
using ACE.Plugin.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace ACE.Plugin.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();

        services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("myS34DSas)@%dfF52345upER$ecr3tK3y034978623")
            );
            var credentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = "plugin!",
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero,
                ValidateAudience = true,
                ValidAudience = "ace web users"
            };
        });



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
        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints((endpoints) =>
        {
            endpoints.MapControllers();
        });



        //var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("%%eCret6545623retkey123"));


        //var tokenValidationParameters = new TokenValidationParameters
        //{
        //    // The signing key must match!
        //    ValidateIssuerSigningKey = true,
        //    IssuerSigningKey = signingKey,
        //    // Validate the JWT Issuer (iss) claim
        //    ValidateIssuer = true,
        //    ValidIssuer = "DemoIssuer",
        //    // Validate the JWT Audience (aud) claim
        //    ValidateAudience = true,
        //    ValidAudience = "DemoAudience",
        //    // Validate the token expiry
        //    ValidateLifetime = true,
        //    // If you want to allow a certain amount of clock drift, set that here:
        //    ClockSkew = TimeSpan.Zero
        //};


        //app.UseAuthentication().UseJwtBearerAuthentication(new JwtBearerOptions
        //{
        //    AutomaticAuthenticate = true,
        //    AutomaticChallenge = true,
        //    TokenValidationParameters = tokenValidationParameters
        //});

    }
}