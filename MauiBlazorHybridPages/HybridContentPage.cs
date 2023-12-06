using MauiBlazorHybridPages.Configuration;
using MauiBlazorHybridPages.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MauiBlazorHybridPages
{
    public class HybridContentPage : ContentPage, IQueryAttributable, IDisposable
    {
        private NavigationManagerInterceptor _navigationInterceptor = null!;
        private NavigationManager _navigationManager = null!;

        private BlazorWebView _webView = null!;
        private Action? _stateHasChanged;
        private string? _razorRouteTemplatePath;

        public void StateHasChanged()
        {
            if (_stateHasChanged != null)
            {
                _stateHasChanged();
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (_razorRouteTemplatePath != null)
            {
                var templatePath = HybridPagesRouteManager.MatchParametersToRouteTemplate(_razorRouteTemplatePath, query);
                _webView.StartPath = templatePath;
            }
        }

        internal void AddHybridWebView(
            IServiceProvider serviceProvider,
            string startupRoute,
            string? razorRouteTemplatePath,
            BlazorWebViewPageOptions webViewOptions)
        {
            _navigationInterceptor = serviceProvider.GetRequiredService<NavigationManagerInterceptor>();

            var baseOptions = serviceProvider.GetRequiredService<BlazorWebViewOptions>();

            var routeComponentParameters = new Dictionary<string, object?>()
            {
                { "AfterOnInitialized", (Action a) => OnInitialized(a) },
                { "OnNavigationManagerInitialized", (NavigationManager nm) => OnNavigationInitialized(nm) }
            };

            _razorRouteTemplatePath = razorRouteTemplatePath;

            // If startup route not passed, we will set later once query parameters come through
            var webView = SetupWebView(
                webViewOptions.HostPage ?? baseOptions.HostPage,
                webViewOptions.Selector ?? baseOptions.Selector,
                startupRoute,
                webViewOptions.ComponentType ?? baseOptions.ComponentType,
                routeComponentParameters);
            Content = _webView = webView;
        }

        private void OnInitialized(Action stateHasChanged)
        {
            _stateHasChanged = stateHasChanged;
        }

        private void OnNavigationInitialized(NavigationManager navigationManager)
        {
            _navigationInterceptor.Attach(navigationManager);
            _navigationManager = navigationManager;
        }

        private static BlazorWebView SetupWebView(
            string hostPage,
            string selector,
            string startPath,
            Type componentType,
            IDictionary<string, object?> routeComponentParameters)
        {
            var webView = new BlazorWebView();
            webView.HostPage = hostPage;
            webView.HorizontalOptions = LayoutOptions.Fill;
            webView.VerticalOptions = LayoutOptions.Fill;
            webView.StartPath = startPath;

            webView.RootComponents.Add(new RootComponent()
            {
                Selector = selector,
                ComponentType = componentType,
                Parameters = routeComponentParameters
            });

            return webView;
        }

        public void Refresh()
        {
            _navigationManager.Refresh();
        }

        public void Dispose()
        {
            _navigationInterceptor.Dispose();
        }
    }
}
