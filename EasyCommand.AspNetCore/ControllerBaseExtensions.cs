using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using EasyCommand.AspNetCore.Exceptions;

namespace EasyCommand.AspNetCore
{
    public static class ControllerBaseExtensions
    {
        private static IServiceScope Scope;

        public static void RegisterScope(IServiceScope scope)
        {
            Scope = scope;
        }

        public static async Task<TResult> ExecuteAsync<TRequest, TResult>(this ControllerBase that, IAsyncAspCommand<TRequest, TResult> command)
        {
            if (that is ControllerBase)
            {
                command.SetController((ControllerBase)that);
            }

            if (CommandRunningConfiguration.Handlers.Length == 0) return await command.ExecuteExternalAsync(default(TRequest));

            return await ExecuteThroughHandlers(command, default(TRequest));
        }

        public static async Task<TResult> ExecuteAsync<TRequest, TResult>(this ControllerBase that, IAsyncAspCommand<TRequest, TResult> command, TRequest request)
        {
            if (that is ControllerBase)
            {
                command.SetController((ControllerBase)that);
            }

            if (CommandRunningConfiguration.Handlers.Length == 0) return await command.ExecuteExternalAsync(request);

            return await ExecuteThroughHandlers(command, request);
        }

        public static TCommand Command<TCommand>(this ControllerBase that) where TCommand : IAsyncCommand
        {
            return Scope.ServiceProvider.GetService<TCommand>();
        }


        private static async Task<TResult> ExecuteThroughHandlers<TRequest, TResult>(IAsyncAspCommand<TRequest, TResult> command, TRequest request)
        {
            var result = default(TResult);
            var handlerIndex = 0;
            var commandType = command.GetType();

            var handlers = RetrieveOrderedHandlers();

            await RunHandlersBeforeMethods<IAsyncAspCommand<TRequest, TResult>, TRequest, TResult>(
                handlers, commandType, request, handlerIndex);

            try
            {
                result = await command.ExecuteExternalAsync(request);
            }
            catch
            {
                var message = $"Failed to run  Execute on {handlers[handlerIndex].GetType()}";
                await RunHandlersAfterMethods
                          (handlers, commandType, result, handlerIndex, message);
                throw;
            }
            finally
            {
                await RunHandlersAfterMethods
                    (handlers, commandType, result, handlerIndex);
            }

            return result;
        }

        private static IAsyncAspCommandHandler[] RetrieveOrderedHandlers()
        {
            var handlers = new IAsyncAspCommandHandler[CommandRunningConfiguration.Handlers.Length];

            for (var i = 0; i < CommandRunningConfiguration.Handlers.Length; i++)
            {
                handlers[i] = (IAsyncAspCommandHandler)Scope.ServiceProvider.GetService(CommandRunningConfiguration.Handlers[i]);
            }

            return handlers;
        }

        private static async Task RunHandlersBeforeMethods<TCommand, TRequest, TResult>(IAsyncAspCommandHandler[] handlers, Type command, TRequest request,
                int handlerIndex)
        {
            for (var i = 0; i < handlers.Length; i++)
            {
                handlerIndex = i;
                try
                {
                    await handlers[i].BeforeExecutionAsync(command, request);
                }
                catch (Exception e)
                {
                    var message = $"Failed to run aBeforeExecutionAsync on {handlers[i].GetType()}";

                    var result = default(TResult);

                    await RunHandlersAfterMethods
                           (handlers, command, result, handlerIndex, message);

                    throw HandlerExecutionException.CreateWhenFailedBeforeCommandExecution(command, handlers[i].GetType(),e);
                }
            }
        }

        private static async Task RunHandlersAfterMethods<TResult>(IAsyncAspCommandHandler[] handlers, Type command, TResult result,
                int handlerIndex, string additionalFailureInformation = null)
        {
            var hasAdditionalInformation = !(additionalFailureInformation is null);

            for (var i = handlerIndex; i >= 0; i--)
            {
                try
                {
                    await handlers[i].AfterExecutionAsync(command, result);
                }
                catch (Exception e)
                {
                    throw HandlerExecutionException.CreateWhenFailedAfterCommandExecution(command, handlers[i].GetType(), e);
                }
            }
        }
    }
}
