using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.StateGroups.Common.Interfaces
{
    public interface IStateGroupsService
    {
        /// <summary>
        /// Asks the browser to look for all not setup state elements and initializes them.
        /// </summary>
        /// <returns></returns>
        Task SetupElements();

        /// <summary>
        ///  Asks the browser to look for all not setup state elements downwards from the given <paramref name="element"/> aswell as the <paramref name="element"/> itself and initializes them.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Task SetupElementsIn(ElementReference element);
    }
}
