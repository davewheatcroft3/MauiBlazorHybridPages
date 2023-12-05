using Microsoft.AspNetCore.Components;

namespace MauiBlazorHybrid.Pages.Navigation
{
    public class NavigationManagerInterceptor : IDisposable
    {
        private readonly HybridPagesRouteManager _routeManager;
        private IDisposable? _disposable;

        public NavigationManagerInterceptor(HybridPagesRouteManager routeManager)
        {
            _routeManager = routeManager;
        }

        internal void Attach(NavigationManager navigationManager)
        {
            _disposable = navigationManager.RegisterLocationChangingHandler(x =>
            {
                var match = _routeManager.MatchFromRazorRoute(x.TargetLocation);
                if (match != null)
                {
                    x.PreventNavigation();
                    _ = NavigateTo(match.Name, match.Parameters);
                }
                return ValueTask.CompletedTask;
            });
        }

        private async Task NavigateTo(string route, Dictionary<string, object> parameters)
        {
            if (parameters.Count == 0)
            {
                await Shell.Current.GoToAsync($"//{route}");
            }
            else
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
