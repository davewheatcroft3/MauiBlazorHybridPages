using MauiBlazorHybrid.Pages.Sample;

namespace MauiBlazorHybrid.Pages;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("ViewWeatherPage", typeof(ViewWeatherPage));
    }
}
