using ACE.Plugin.Crypto.Managers;
using ACE.Plugin.Web.Common;
using ACE.Plugin.Web.Model;
using ACE.Plugin.Web.Model.Admin;
using ACE.Server.Plugin;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ACE.Plugin.Web.Managers
{
    /// <summary>
    /// WebManager handles the WebHost.  WebHost responds to HTTP and HTTPS requests.
    /// </summary>
    public static class WebManager
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static CancellationTokenSource Halter = new CancellationTokenSource();

        public static void PreInitialize()
        {
            // invoke dependency resolution while Web DLLs have precedence
            Type type = null;
            type = typeof(WebHostBuilder);
            //type = typeof(KestrelStartup);
            type = typeof(X509Certificate2);
            type = typeof(RSA);
            type = typeof(WebHostDefaults);
            // type = typeof(StatelessAuthenticationConfiguration);
            type = typeof(ClaimsIdentity);
            type = typeof(IWebHost);
            type = typeof(IWebHostBuilder);
            type = typeof(WebHost);
            type = typeof(WebHostBuilder);
            // type = typeof(TinyIoCContainer);
            // type = typeof(NancyContext);
            // type = typeof(IPipelines);
            // type = typeof(INancyEnvironment);
            // type = typeof(Response);
            // type = typeof(NancyModule);
            type = typeof(AbstractValidator<AdminCommandRequestModel>);
            type = typeof(ControllerBase);
            type = typeof(IConfigurationBuilder);
            // type = typeof(ModuleExtensions);

            // loads Nancy.Validation.FluentValidation assembly into the current app domain so that the patched DependencyContextAssemblyCatalog will catalog it upon startup
            // var z = new DefaultFluentAdapterFactory(null);

            Gate gate = Gate.Instance; // spin up gate threads
            SetPerch();
        }

        private static Perch Perch { get; set; } = null;

        public static void SetPerch()
        {
            string listeningHost = WebConfigManager.Config.Host;
            ushort listeningPort = WebConfigManager.Config.Port;
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
        public static async Task RunAsync()
        {
            if (Perch == null)
            {
                return;
            }
            var certs = Path.Combine(PluginManager.PathToACEFolder, "certificates");


            var builder = new WebHostBuilder();

            builder.UseKestrel();
            builder.UseUrls($"http://{Perch.Address}:{Perch.Port}");
            if (CertificateManager.CertificateWeb != null)
            {
                builder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureEndpointDefaults(listenOptions =>
                    {
                        listenOptions
                            .UseHttps(new HttpsConnectionAdapterOptions()
                            {
                                ServerCertificate = new X509Certificate2(CertificateManager.CertificateWeb)
                            });
                    });
                });
            }

            builder.UseStartup<Startup>();
            var host = builder.Build();

            log.Info($"Binding web services to {Perch.Address}:{Perch.Port}");
            await host.RunAsync(Halter.Token);
        }

        public static void Run()
        {
            WebGlobal.ResultOfHostRunSink.SetResult(true); // todo: tie this into the host startup pipeline
            Thread th = new Thread(new ThreadStart(async () =>
            {
                try
                {
                    await RunAsync();
                }
                catch (Exception ex)
                {
                    log.FatalFormat("WebHost has thrown: {0}", ex.Message, ex);
                }
            }));
            th.Name = "WebHost";
            th.Start();
        }

        /// <summary>
        /// stop the web host, grace lasts for 1 second, then remaining requests are jettisoned.
        /// </summary>
        public static void Shutdown()
        {
            Gate.Shutdown();
            Halter.Cancel(); // terminate web host
        }
    }
}