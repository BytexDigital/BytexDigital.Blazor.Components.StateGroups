using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using static BytexDigital.Blazor.Components.StateGroups.StateElement.StateIndicatorOptionsBuilder;

namespace BytexDigital.Blazor.Components.StateGroups
{
    public partial class StateGroup : ComponentBase
    {
        [Inject]
        public IJSRuntime JsRuntime { get; private set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// True if you want this state group to inherit it's status from a parent group.
        /// <para>Without defining a <see cref="ParentGroupId"/>, this group will attach to the nearest group upwards in the DOM.</para>
        /// </summary>
        [Parameter]
        public bool InheritState { get; set; } = true;

        /// <summary>
        /// If set, will make this StateGroup available as a cascading parameter only under the given identifier.
        /// </summary>
        [Parameter]
        public string? CascadeAs { get; set; }

        /// <summary>
        /// Explicitly defines which state group is the parent group defined by the id.
        /// </summary>
        [Parameter]
        public string ParentGroupId { get; set; }

        /// <summary>
        /// Id of the state group.
        /// </summary>
        [Parameter]
        public string Id { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Id == null)
            {
                Id = Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Sets the group to a busy state.
        /// </summary>
        /// <returns></returns>
        public async Task SetBusyAsync()
        {
            await JsRuntime.InvokeVoidAsync("StateIndicators.EnableGroup", null, Id);
        }

        /// <summary>
        /// Sets the group to a idle state.
        /// </summary>
        /// <returns></returns>
        public async Task SetIdleAsync()
        {
            await JsRuntime.InvokeVoidAsync("StateIndicators.DisableGroup", Id);
        }
    }
}