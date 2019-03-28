using ACE.Plugin.Crypto.Managers;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading;

namespace ACE.Plugin.WebAPI
{
    internal static class WebHost
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Thread hostThread = null;
        private static IWebHost host = null;
        public static void Run(IPAddress listenAt, int port)
        {
            if (hostThread != null)
            {
                return;
            }
            if (CertificateManager.CertificateWebApi == null)
            {
                log.Fatal("Key and certificate bundle is unavailable.  Aborting WebAPI hosting.");
                WebAPIGlobal.ResultOfHostRunSink.SetResult(false);
                return;
            }
            hostThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    host = new WebHostBuilder()
                        .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
                        .UseKestrel(options =>
                        {
                            options.Listen(listenAt, port, listenOptions =>
                            {
                                listenOptions.UseHttps(CertificateManager.CertificateWebApi);
                            });
                        })
                        .UseStartup<KestrelStartup>()
                        .Build();
                    host.Run();
                }
                catch (Exception ex)
                {
                    log.Fatal("WebHost failure.", ex);
                    if (WebAPIGlobal.ResultOfHostRunSink.Task.Status == System.Threading.Tasks.TaskStatus.WaitingForActivation)
                    {
                        WebAPIGlobal.ResultOfHostRunSink.SetResult(false);
                    }
                }
            }));
            //hostThread.SetApartmentState(ApartmentState.STA); // linux: System.PlatformNotSupportedException: COM interop is not supported on this platform.
            hostThread.Priority = ThreadPriority.BelowNormal;
            hostThread.Start();
        }
        public static void Shutdown()
        {
            host.StopAsync(TimeSpan.FromSeconds(1000)).RunSynchronously();
        }
    }
}
