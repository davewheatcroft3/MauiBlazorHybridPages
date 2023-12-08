namespace MauiBlazorHybridPages.Builders
{
    public class AppShellNavigationOptions
    {
        /// <summary>
        /// Defaults to true, register via Routing.RegisterRoute with AppShell.
        /// </summary>
        public bool RegisterAppShellRoute { get; set; } = true;

        /// <summary>
        /// Defaults to null. If not empty, will prefix the pushed App Shell route with the given value.
        /// E.g. Adding /'s to the routes for AppShell.
        /// </summary>
        public string? PrefixAppShellRoute { get; set; }
    }


    public interface IPageRouteBuilder
    {
        /// <summary>
        /// Register the page being built with the App Shell.
        /// NOTE: this will call Microsoft.Maui.Controls.Routing.RegisterRoute for the route name and page type.
        /// </summary>
        /// <param name="appShellRouteName">Name of the app shell route</param>
        /// <param name="appShellNavigationOptions">Defaults to null, alter additional options for app shell navigation.</param>
        void WithAppShellNavigatableRoute(string appShellRouteName, Action<AppShellNavigationOptions>? appShellNavigationOptions = null);

        /// <summary>
        /// Register the page being built with the App Shell AND add matching functionality so any navigation requests from
        /// inside Blazor will be forward to app navigation instead.
        /// NOTE: this will call Microsoft.Maui.Controls.Routing.RegisterRoute for the route name and page type.
        /// </summary>
        /// <param name="appShellRouteName">Name of the app shell route</param>
        /// <param name="razorTemplateRoute">The route (including {} parameters) defined for the razor page</param>
        /// <param name="appShellNavigationOptions">Defaults to null, alter additional options for app shell navigation.</param>
        void WithAppShellNavigatableRoute(string appShellRouteName, string razorTemplateRoute, Action<AppShellNavigationOptions>? appShellNavigationOptions = null);
    }

    public class PageRouteBuilder : IPageRouteBuilder
    {
        private readonly PageBuilderDescriptor _pageBuilderDescriptor;

        internal PageRouteBuilder(PageBuilderDescriptor pageBuilderDescriptor)
        {
            _pageBuilderDescriptor = pageBuilderDescriptor;
        }
        public void WithAppShellNavigatableRoute(string appShellRouteName, Action<AppShellNavigationOptions>? appShellNavigationOptions = null)
        {
            _pageBuilderDescriptor.AppShellRouteName = appShellRouteName;

            var options = new AppShellNavigationOptions();
            if (appShellNavigationOptions != null)
            {
                appShellNavigationOptions(options);
            }
            _pageBuilderDescriptor.AppShellNavigationOptions = options;
        }

        public void WithAppShellNavigatableRoute(string appShellRouteName, string razorTemplateRoute, Action<AppShellNavigationOptions>? appShellNavigationOptions = null)
        {
            WithAppShellNavigatableRoute(appShellRouteName, appShellNavigationOptions);
            _pageBuilderDescriptor.RazorRouteTemplatePath = razorTemplateRoute;
        }
    }
}