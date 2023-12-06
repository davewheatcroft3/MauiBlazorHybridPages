using Microsoft.Extensions.Logging;

namespace MauiBlazorHybridPages.Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddHybridPages(typeof(RazorEntryPoints.Routes))
              .WithAppShellNavigation()
              .WithPages(builder =>
              {
                  builder.AddTransient<HomePage>("Home", "/");
                  builder.AddTransient<CounterPage>("Counter", "/counter");
                  builder.AddTransient<WeatherPage>("Weather", "/weather");
                  builder
                    .AddTransient<ViewWeatherPage>("View Weather")
                    .WithAppShellNavigatableRoute("ViewWeatherPage", "/weather/view/{row:int}");
              });

            return builder.Build();
        }
    }
}
