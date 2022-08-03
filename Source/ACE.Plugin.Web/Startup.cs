using ACE.Database;
using ACE.Plugin.Web.Logging;
using ACE.Plugin.Web.Model.Character;
using ACE.Plugin.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
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

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

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

            Endpoints.GetCharacters(endpoints);

            //for example
            //endpoints.MapPut("customers/block/{customerId}", async ([FromRoute] string customerId, [FromBody] BlockCustomer blockCustomer, [FromServices] ICustomersRepository customersRepository) =>
            //{
            //    var customer = await _customersRepository.Get(command.CustomerId);
            //    customer.Block(command.Reason);
            //    await _customersRepository.Save(customer);
            //    return Results.Ok();
            //});



        });

    }
}