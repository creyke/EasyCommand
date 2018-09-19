using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EasyCommand.AspNetCore
{
    public static class ControllerBaseExtensions
    {
        private static IServiceScope Scope;

        public static void RegisterScope(IServiceScope scope)
        {
            Scope = scope;
        }

        public static async Task<TResult> ExecuteAsync<TRequest, TResult>(this object that, IAsyncAspCommand<TRequest, TResult> command)
        {
            if (that is ControllerBase)
            {
                command.SetController((ControllerBase)that);
            }

            await ExecuteBeforeCommand(); 

            return await command.ExecuteExternalAsync(default(TRequest));
        }

        public static async Task<TResult> ExecuteAsync<TRequest, TResult>(this object that, IAsyncAspCommand<TRequest, TResult> command, TRequest request)
        {
            if (that is ControllerBase)
            {
                command.SetController((ControllerBase)that);
            }

            await ExecuteBeforeCommand();

            return await command.ExecuteExternalAsync(request);
        }

        public static TCommand Command<TCommand>(this object that) where TCommand : IAsyncCommand
        {
            return Scope.ServiceProvider.GetService<TCommand>();
        }

        private static async Task ExecuteBeforeCommand()
        {
            var allExecutingBefore = Scope.ServiceProvider.GetServices<IRunBeforeCommand>()?.ToArray() ?? new IRunBeforeCommand[0];

            if (allExecutingBefore.Any())
            {
                await Task.WhenAll(allExecutingBefore.Select(executingBefore =>
                    executingBefore.BeforeExecutionOfCommand()));
            }
        }
    }
}
