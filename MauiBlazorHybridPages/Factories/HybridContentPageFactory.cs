using MauiBlazorHybridPages.Configuration;

namespace MauiBlazorHybridPages.Factories
{
    public static class HybridContentPageFactory
    {
        public static object Generate(
            Type type,
            IServiceProvider serviceProvider,
            string title,
            string razorStartPath,
            string? razorRouteTemplatePath,
            Action<BlazorWebViewPageOptions>? configureWebViewOptions)
        {
            // TODO: allow nested inheritance...
            if (type.BaseType != typeof(HybridContentPage))
            {
                throw new Exception($"Page {type.Name} doesnt inherit from HybridContentPage");
            }

            var webViewOptions = new BlazorWebViewPageOptions();

            if (configureWebViewOptions != null)
            {
                configureWebViewOptions(webViewOptions);
            }

            // TODO: look for constructor params by reflection and try and instantiate...
            var instance = (HybridContentPage)Activator.CreateInstance(type)!;

            instance.AddHybridWebView(serviceProvider, razorStartPath, razorRouteTemplatePath, webViewOptions);

            instance.Title = title;

            return instance;
        }
    }
}