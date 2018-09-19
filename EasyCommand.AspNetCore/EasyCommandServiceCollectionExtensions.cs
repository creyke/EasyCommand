using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace EasyCommand.AspNetCore
{
    public static class EasyCommandServiceCollectionExtensions
    {
        public static IServiceCollection AddEasyCommand(this IServiceCollection services, IStartup assemblyObject, Action<CommandRunningConfiguration> buildConfiguration = null)
        {
            var types = assemblyObject.GetType().Assembly.GetTypes();

            return SetupEasyCommand(services, types, buildConfiguration);

        }

        public static IServiceCollection AddEasyCommand(this IServiceCollection services, Action<CommandRunningConfiguration> buildConfiguration = null)
        {
            var types = Assembly.GetCallingAssembly().GetTypes();

            return services.SetupEasyCommand(types, buildConfiguration);
        }

        private static IServiceCollection SetupEasyCommand(this IServiceCollection services, IEnumerable<Type> allTypes, Action<CommandRunningConfiguration> buildConfiguration = null)
        {   
            services.RegisterCommands(allTypes);

            if (buildConfiguration is null)
                return services;

            var configuration = new CommandRunningConfiguration();
            buildConfiguration(configuration);

            if (configuration.toRunBeforeCommands.Any())
                RegisterRunningBeforeCommands(services, configuration);

            return services;
        }

        private static void RegisterCommands(this IServiceCollection services, IEnumerable<Type> allTypes)
        {
            var types = allTypes.Where(t =>
                typeof(IAsyncCommand).IsAssignableFrom(t));

            foreach (var type in types)
            {
                services.AddTransient(type);
            }
        }

        private static void RegisterRunningBeforeCommands(this IServiceCollection services, CommandRunningConfiguration configuration)
        {
            foreach (var runBefore in configuration.toRunBeforeCommands)
            {
                services.AddTransient(typeof(IRunBeforeCommand), runBefore);
            }
        }
    }
}
