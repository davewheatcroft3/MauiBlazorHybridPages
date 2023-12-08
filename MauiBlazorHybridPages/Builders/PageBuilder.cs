using MauiBlazorHybridPages.Configuration;
using MauiBlazorHybridPages.Factories;
using MauiBlazorHybridPages.Navigation;

namespace MauiBlazorHybridPages.Builders
{
    public interface IPageBuilder
    {
        IPageRouteBuilder AddTransient<T>(
                string title,
                string startupRoute = "/",
                Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
            where T : HybridContentPage;

        IPageRouteBuilder AddScoped<T>(
                string title,
                string startupRoute = "/",
                Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
            where T : HybridContentPage;

        IPageRouteBuilder AddSingleton<T>(
                string title,
                string startupRoute = "/",
                Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
            where T : HybridContentPage;
    }

    internal class PageBuilderDescriptor
    {
        public Type Type { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; }
        public string Title { get; set; }
        public string? AppShellRouteName { get; set; }
        public string RazorStartPath { get; set; }
        public string? RazorRouteTemplatePath { get; set; }
        public AppShellNavigationOptions AppShellNavigationOptions { get; set; } = new();
        public Action<BlazorWebViewPageOptions>? ConfigureWebViewOptions { get; set; }

        internal PageBuilderDescriptor(
            Type type,
            ServiceLifetime serviceLifetime,
            string title,
            string razorStartPath,
            Action<BlazorWebViewPageOptions>? configureWebViewOptions)
        {
            Type = type;
            ServiceLifetime = serviceLifetime;
            Title = title;
            RazorStartPath = razorStartPath;
            ConfigureWebViewOptions = configureWebViewOptions;
        }
    }

    public class PageBuilder : IPageBuilder
    {
        private readonly IServiceCollection _services;
        private readonly HybridPagesRouteManager _routeManager;

        private List<PageBuilderDescriptor> _pageDescriptors = new();

        public PageBuilder(IServiceCollection services, HybridPagesRouteManager routeManager)
        {
            _services = services;
            _routeManager = routeManager;
        }

        public IPageRouteBuilder AddTransient<T>(
                 string title,
                 string startupRoute = "/",
                 Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
             where T : HybridContentPage
        {
            var descriptor = Add<T>(ServiceLifetime.Transient, title, startupRoute, configureWebViewOptions);
            return new PageRouteBuilder(descriptor);
        }

        public IPageRouteBuilder AddScoped<T>(
                string title,
                string startupRoute = "/",
                Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
            where T : HybridContentPage
        {
            var descriptor = Add<T>(ServiceLifetime.Scoped, title, startupRoute, configureWebViewOptions);
            return new PageRouteBuilder(descriptor);
        }

        public IPageRouteBuilder AddSingleton<T>(
                string title,
                string startupRoute = "/",
                Action<BlazorWebViewPageOptions>? configureWebViewOptions = null)
            where T : HybridContentPage
        {
            var descriptor = Add<T>(ServiceLifetime.Singleton, title, startupRoute, configureWebViewOptions);
            return new PageRouteBuilder(descriptor);
        }

        public void Build()
        {
            foreach (var descriptor in _pageDescriptors)
            {
                BuildDescriptor(descriptor);
            }
        }

        private PageBuilderDescriptor Add<T>(
            ServiceLifetime serviceLifetime,
            string title,
            string razorStartPath,
            Action<BlazorWebViewPageOptions>? configureWebViewOptions)
            where T : HybridContentPage
        {
            var descriptor = new PageBuilderDescriptor(typeof(T), serviceLifetime, title, razorStartPath, configureWebViewOptions);
            _pageDescriptors.Add(descriptor);
            return descriptor;
        }

        private void BuildDescriptor(PageBuilderDescriptor descriptor)
        {
            _services.Add(new ServiceDescriptor(
                descriptor.Type,
                (sp) => HybridContentPageFactory.Generate(
                    descriptor.Type,
                    sp,
                    descriptor.Title,
                    descriptor.RazorStartPath,
                    descriptor.RazorRouteTemplatePath,
                    descriptor.ConfigureWebViewOptions),
                    descriptor.ServiceLifetime));

            if (descriptor.AppShellRouteName != null)
            {
                if (descriptor.AppShellNavigationOptions.RegisterAppShellRoute)
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    Routing.RegisterRoute(descriptor.AppShellRouteName, descriptor.Type);
#pragma warning restore CA1416 // Validate platform compatibility
                }

                if (descriptor.RazorRouteTemplatePath != null)
                {
                    _routeManager.Register(
                        descriptor.AppShellRouteName,
                        descriptor.RazorRouteTemplatePath,
                        descriptor.AppShellNavigationOptions.PrefixAppShellRoute);
                }
            }
        }
    }
}