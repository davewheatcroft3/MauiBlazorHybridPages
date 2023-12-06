namespace MauiBlazorHybridPages.Builders
{
    public interface IPageRouteBuilder
    {
        /// <summary>
        /// Register the page being built with the App Shell.
        /// NOTE: this will call Microsoft.Maui.Controls.Routing.RegisterRoute for the route name and page type.
        /// </summary>
        /// <param name="appShellRouteName">Name of the app shell route</param>
        /// <param name="registerAppShellRoute">Defaults to true, register via Routing.RegisterRoute with AppShell.</param>
        void WithAppShellNavigatableRoute(string appShellRouteName, bool registerAppShellRoute = true);

        /// <summary>
        /// Register the page being built with the App Shell AND add matching functionality so any navigation requests from
        /// inside Blazor will be forward to app navigation instead.
        /// NOTE: this will call Microsoft.Maui.Controls.Routing.RegisterRoute for the route name and page type.
        /// </summary>
        /// <param name="appShellRouteName">Name of the app shell route</param>
        /// <param name="razorTemplateRoute">The route (including {} parameters) defined for the razor page</param>
        /// <param name="registerAppShellRoute">Defaults to true, register via Routing.RegisterRoute with AppShell.</param>
        void WithAppShellNavigatableRoute(string appShellRouteName, string razorTemplateRoute, bool registerAppShellRoute = true);
    }

    public class PageRouteBuilder : IPageRouteBuilder
    {
        private readonly PageBuilderDescriptor _pageBuilderDescriptor;

        internal PageRouteBuilder(PageBuilderDescriptor pageBuilderDescriptor)
        {
            _pageBuilderDescriptor = pageBuilderDescriptor;
        }
        public void WithAppShellNavigatableRoute(string appShellRouteName, bool registerAppShellRoute = true)
        {
            _pageBuilderDescriptor.AppShellRouteName = appShellRouteName;
            _pageBuilderDescriptor.RegisterRoute = registerAppShellRoute;
        }

        public void WithAppShellNavigatableRoute(string appShellRouteName, string razorTemplateRoute, bool registerAppShellRoute = true)
        {
            WithAppShellNavigatableRoute(appShellRouteName, registerAppShellRoute);
            _pageBuilderDescriptor.RazorRouteTemplatePath = razorTemplateRoute;
        }
    }
}