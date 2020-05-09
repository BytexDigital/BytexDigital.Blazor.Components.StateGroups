using BytexDigital.Blazor.Components.StateGroups.Common;
using BytexDigital.Blazor.Components.StateGroups.Common.Interfaces;
using BytexDigital.Blazor.Components.StateGroups.Common.Services;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace BytexDigital.Blazor.Components.StateGroups.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateGroups(this IServiceCollection services)
        {
            AddStateGroups(services, options => { });

            return services;
        }

        public static IServiceCollection AddStateGroups(this IServiceCollection services, Action<StateGroupsOptions> optionsConfiguration)
        {
            var options = new StateGroupsOptions
            {

            };

            optionsConfiguration.Invoke(options);

            services.AddSingleton(options);
            services.AddScoped<IStateGroupsService, StateGroupsService>();

            return services;
        }
    }
}
