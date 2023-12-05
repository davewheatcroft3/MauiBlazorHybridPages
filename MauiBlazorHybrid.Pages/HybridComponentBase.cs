using Microsoft.AspNetCore.Components;

namespace MauiBlazorHybrid.Pages
{
    public class HybridComponentBase : ComponentBase
    {
        [Parameter]
        public Action<Action>? AfterOnInitialized { get; set; }

        [Parameter]
        public Action<NavigationManager>? OnNavigationManagerInitialized { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (AfterOnInitialized != null)
            {
                AfterOnInitialized(() => StateHasChanged());
            }
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (OnNavigationManagerInitialized != null)
            {
                OnNavigationManagerInitialized(NavigationManager);
            }
        }
    }
}