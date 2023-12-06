# MAUI Blazor Hybrid Pages
Helper library for minimizing Razor in your hybrid MAUI app project

## Installation

### Required Steps
1. Install Nuget package
```
Install-Package MauiBlazorHybridPages
```

2. In your Program.cs add
```cs
 builder.Services.AddHybridPages(typeof(RazorEntryPoints.Routes))
    .WithAppShellNavigation()
    .WithPages(builder =>
    {
        builder.AddTransient<HomePage>("Home", "/");
        builder.AddTransient<CounterPage>("Counter", "/counter");
        builder.AddTransient<WeatherPage>("Weather", "/weather");
    });
```

3. Define your pages
```cs
public class HomePage : HybridContentPage { }
public class CounterPage : HybridContentPage { }
public class WeatherPage : HybridContentPage { }
```

4. Make sure your routes razor file inherits from HybridComponentBase
```cs
@inherits HybridComponentBase

<Router AppAssembly="@typeof(MauiProgram).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
</Router>
```

###Allowing for Razor parameters via Navigation
You can add navigation support for parameters by adding this the page builder in the WithPages scope:
```cs
    builder
    .AddTransient<ViewWeatherPage>("View Weather")
    .WithAppShellNavigatableRoute("/weather/view/{row:int}", "ViewWeatherPage");

```
