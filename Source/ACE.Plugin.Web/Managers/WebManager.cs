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
        private static Thread? webHostStartThread;
        private static IWebHost? webHost;
        private static ListenConfiguration? ListenConfiguration { get; set; } = null;
        private static readonly CancellationTokenSource webHostHalter = new();

        private static void PreloadTypes()
        {
            // invoke dependency resolution while Web DLLs have precedence
            Type? type = null;
            type = typeof(WebHostBuilder);
            type = typeof(X509Certificate2);
            type = typeof(RSA);
            type = typeof(WebHostDefaults);
            type = typeof(ClaimsIdentity);
            type = typeof(IWebHost);
            type = typeof(IWebHostBuilder);
            type = typeof(WebHost);
            type = typeof(WebHostBuilder);
            type = typeof(AbstractValidator<AdminCommandRequestModel>);
            type = typeof(ControllerBase);
            type = typeof(IConfigurationBuilder);

            // loads Nancy.Validation.FluentValidation assembly into the current app domain so that the patched DependencyContextAssemblyCatalog will catalog it upon startup
            // var z = new DefaultFluentAdapterFactory(null);
        }
        public static void SetWebServicesListenConfiguration()
        {
            string listeningHost = WebConfigManager.Config.Host;
            ushort listeningPort = WebConfigManager.Config.Port;
            if (!IPAddress.TryParse(listeningHost, out IPAddress listenAt))
            {
                string msg = $"Unable to parse IP address {listeningHost}";
                log.Error(msg);
                throw new Exception(msg);
            }
            ListenConfiguration = new ListenConfiguration() { Address = listenAt, Port = listeningPort };
        }
        public static void PreInitialize()
        {
            PreloadTypes();
            Gate gate = Gate.Instance;
            SetWebServicesListenConfiguration();
        }
        /// <summary>
        /// start the web host
        /// </summary>
        public static void StartWebServices()
        {
            if (webHostStartThread != null)
            {
                return;
            }
            if (ListenConfiguration == null)
            {
                log.Fatal("Invalid web host addr:port listen configuration.");
                WebGlobal.ResultOfHostRunSink.SetResult(false);
                return;
            }
            try
            {
                if (ListenConfiguration == null)
                {
                    return;
                }
                var certs = Path.Combine(PluginManager.PathToACEFolder, "certificates");
                var builder = new WebHostBuilder();
                builder.UseKestrel();
                builder.UseUrls($"http://{ListenConfiguration.Address}:{ListenConfiguration.Port}");
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
                webHost = builder.Build();
                WebGlobal.ResultOfHostRunSink.SetResult(true);
            }
            catch (Exception ex)
            {
                log.Fatal("WebHost build failure.", ex);
                if (WebGlobal.ResultOfHostRunSink.Task.Status == TaskStatus.WaitingForActivation)
                {
                    WebGlobal.ResultOfHostRunSink.SetResult(false);
                }
            }
            webHostStartThread = new Thread(new ThreadStart(async () =>
            {
                try
                {
                    log.Info($"Binding web services to {ListenConfiguration.Address}:{ListenConfiguration.Port}");
                    await webHost.RunAsync(webHostHalter.Token);
                }
                catch (Exception ex)
                {
                    log.Fatal("WebHost failure.", ex);
                    if (WebGlobal.ResultOfHostRunSink.Task.Status == TaskStatus.WaitingForActivation)
                    {
                        WebGlobal.ResultOfHostRunSink.SetResult(false);
                    }
                }
            }));
            webHostStartThread.Priority = ThreadPriority.BelowNormal;
            webHostStartThread.Start();
        }
        /// <summary>
        /// stop the web host
        /// </summary>
        public static void StopWebServices()
        {
            Gate.Shutdown();
            webHostHalter.Cancel();
        }
    }
}