using BytexDigital.Blazor.Components.StateGroups.Common.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.StateGroups.Common.Services
{
    public class StateGroupsService : IStateGroupsService
    {
        private readonly StateGroupsOptions _options;
        private readonly IJSRuntime _jsRuntime;

        public StateGroupsService(StateGroupsOptions options, IJSRuntime jsRuntime)
        {
            _options = options;
            _jsRuntime = jsRuntime;
        }

        public async Task SetupElements()
        {
            await _jsRuntime.InvokeVoidAsync("window.StateIndicators.SetupAllElements");
        }

        public async Task SetupElementsIn(ElementReference element)
        {
            await _jsRuntime.InvokeVoidAsync("window.StateIndicators.SetupElementsIn", element);
        }
    }
}
