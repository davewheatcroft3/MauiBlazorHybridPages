using MauiBlazorHybrid.Pages.Navigation;

namespace MauiBlazorHybrid.Pages.Builders
{
    public interface IHybridPagesBuilder
    {
        IHybridPagesBuilder WithAppShellNavigation();

        IServiceCollection WithPages(Action<IPageBuilder> builder);
    }

    public class HybridPagesBuilder : IHybridPagesBuilder
    {
        private readonly IServiceCollection _services;
        private readonly HybridPagesRouteManager _routeManager;

        public HybridPagesBuilder(IServiceCollection services, HybridPagesRouteManager routeManager)
        {
            _services = services;
            _routeManager = routeManager;
        }

        public IHybridPagesBuilder WithAppShellNavigation()
        {
            // TODO: currently only app shell navigation supported - add other options!
            _services.AddSingleton(sp =>
            {
                return new NavigationManagerInterceptor(_routeManager);
            });
            return this;
        }

        public IServiceCollection WithPages(Action<IPageBuilder> builder)
        {
            var pageBuilder = new PageBuilder(_services, _routeManager);
            builder(pageBuilder);
            pageBuilder.Build();
            return _services;
        }
    }
}