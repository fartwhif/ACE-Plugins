using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;

namespace ACE.Plugin.WebAPI
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        public NancyBootstrapper() { }

        // Override with a valid password (albeit a really really bad one!)
        // to enable the diagnostics dashboard
        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);

            base.Configure(environment);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            StatelessAuthentication.Enable(pipelines, BasicAuth.GetAuthCfg());
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            //Conventions.ViewLocationConventions.Add((viewName, model, viewLocationContext) => string.Concat("Web/Views/", viewName));
            //WebAPIGlobal.ResultOfInitSink.SetResult(true);
            WebAPIGlobal.ResultOfHostRunSink.SetResult(true);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
        }

    }
}
