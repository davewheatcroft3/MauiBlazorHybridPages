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
            Func<IServiceProvider, Task<IDictionary<string, object>>>? setRouteOnGenerate,
            Action<BlazorWebViewPageOptions>? configureWebViewOptions)
        {
            if (type.BaseType == null || !type.IsSubclassOf(typeof(HybridContentPage)))
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

            if (setRouteOnGenerate != null)
            {
                var dict = setRouteOnGenerate(serviceProvider).ConfigureAwait(false).GetAwaiter().GetResult();
                instance.ApplyQueryAttributes(dict);
            }

            instance.Title = title;

            return instance;
        }
    }
}