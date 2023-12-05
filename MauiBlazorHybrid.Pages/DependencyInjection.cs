using MauiBlazorHybrid.Pages.Builders;
using MauiBlazorHybrid.Pages.Configuration;
using MauiBlazorHybrid.Pages.Navigation;

namespace MauiBlazorHybrid.Pages
{
    public static class DependencyInjection
    {
        public static IHybridPagesBuilder AddHybridPages(this IServiceCollection services, Type componentType)
        {
            return _AddHybridPages(services, componentType, null);
        }

        public static IHybridPagesBuilder AddHybridPages(
            this IServiceCollection services,
            Type componentType,
            Action<BlazorWebViewOptions> configureOptions)
        {
            return _AddHybridPages(services, componentType, configureOptions);
        }

        private static IHybridPagesBuilder _AddHybridPages(
            IServiceCollection services,
            Type componentType,
            Action<BlazorWebViewOptions>? configureOptions)
        {
            var options = new BlazorWebViewOptions() { ComponentType = componentType };
            if (configureOptions != null)
            {
                configureOptions(options);
            }
            services.AddSingleton<BlazorWebViewOptions>(_ => options);

            var routeManager = new HybridPagesRouteManager();
            services.AddSingleton<HybridPagesRouteManager>(routeManager);

            return new HybridPagesBuilder(services, routeManager);
        }
    }
}
