using ACE.Plugin.WebAPI.Common;
using ACE.Plugin.WebAPI.Model;
using ACE.Plugin.WebAPI.Model.Admin;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.ModelBinding;
using Nancy.TinyIoc;
using Nancy.Validation.FluentValidation;
using System;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ACE.Plugin.WebAPI.Managers
{
    /// <summary>
    /// Web Manager handles the web host which honors HTTP and HTTPS external web requests
    /// </summary>
    public static class WebManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void PreInitialize()
        {
            // invoke dependency resolution while WebAPI DLLs have precedence
            Type type = null;
            type = typeof(WebHostBuilder);
            type = typeof(KestrelStartup);
            type = typeof(X509Certificate2);
            type = typeof(RSA);
            type = typeof(WebHostDefaults);
            type = typeof(StatelessAuthenticationConfiguration);
            type = typeof(ClaimsIdentity);
            type = typeof(IWebHost);
            type = typeof(WebHost);
            type = typeof(TinyIoCContainer);
            type = typeof(NancyContext);
            type = typeof(IPipelines);
            type = typeof(INancyEnvironment);
            type = typeof(Response);
            type = typeof(NancyModule);
            type = typeof(AbstractValidator<AdminCommandRequestModel>);
            type = typeof(ModuleExtensions);

            // loads Nancy.Validation.FluentValidation assembly into the current app domain so that the patched DependencyContextAssemblyCatalog will catalog it upon startup
            var z = new DefaultFluentAdapterFactory(null);

            Gate gate = Gate.Instance; // spin up gate threads
            SetPerch();
        }

        private static Perch Perch { get; set; } = null;
        public static void SetPerch()
        {
            string listeningHost = WebAPIConfigManager.Config.Host;
            ushort listeningPort = WebAPIConfigManager.Config.Port;
            if (!IPAddress.TryParse(listeningHost, out IPAddress listenAt))
            {
                string msg = $"Unable to parse IP address {listeningHost}";
                log.Error(msg);
                throw new Exception(msg);
            }
            Perch = new Perch() { Address = listenAt, Port = listeningPort };
        }

        /// <summary>
        /// start the web host
        /// </summary>
        public static void Initialize()
        {
            if (Perch == null)
            {
                return;
            }
            try
            {
                log.Info($"Binding WebApi to {Perch.Address}:{Perch.Port}");
                WebHost.Run(Perch.Address, Perch.Port);
            }
            catch (Exception ex)
            {
                log.FatalFormat("WebHost has thrown: {0}", ex.Message, ex);
            }
        }

        /// <summary>
        /// stop the web host, grace lasts for 1 second, then remaining requests are jettisoned.
        /// </summary>
        public static void Shutdown()
        {
            Gate.Shutdown();
            WebHost.Shutdown();
        }
    }
}
