namespace MauiBlazorHybridPages.Configuration
{
    public class BlazorWebViewOptions
    {
        public string HostPage { get; init; } = "wwwroot/index.html";

        public string Selector { get; init; } = "#app";

        public Type ComponentType { get; init; } = null!;
    }

    public class BlazorWebViewPageOptions
    {
        public string? HostPage { get; init; }

        public string? Selector { get; init; }

        public Type? ComponentType { get; init; }
    }
}
