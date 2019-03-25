using EasyCommand.AspNetCore.Exceptions;
using EasyCommand.AspNetCore.Tests.Commands;
using EasyCommand.AspNetCore.Tests.Handler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EasyCommand.AspNetCore.Tests
{
    public class EasyCommandServiceCollectionExtensionsTests : EasyCommandServiceCollectionExtensionsTestsBase
    {
        [Fact]
        public void SuccessfullyRegistersCommand_NoOptions()
        {
            var services = CreateServiceProvider(
                    t => t.AddEasyCommand());

            var command = services.GetService<ExampleCommandWithRequestAndResult>();

            Assert.NotNull(command);
        }

        [Fact]
        public void SuccessfullyRegistersCommand_WithObject()
        {
            var services = CreateServiceProvider(
                    t => t.AddEasyCommand(new Startup(null)));

            var command = services.GetService<ExampleCommandWithRequestAndResult>();

            Assert.NotNull(command);
        }

        [Fact]
        public void SuccessfullyRegistersCustomHandler()
        {
            var services = CreateServiceProvider(
                    t => t.AddEasyCommand(c=>c.AddControllerCommandHandler<ExampleHandler>()));

            var runBeforeComamnds = services.GetServices<ExampleHandler>();

            Assert.NotNull(runBeforeComamnds);
            Assert.Contains(runBeforeComamnds, t => t.GetType() == typeof(ExampleHandler));
        }

        [Fact]
        public async Task CustomHandlerThrowsCorrectExceptionBefore()
        {
            await AssertHandlerThrowsCorrectException<FailBeforeCommandHandler, 
                ExampleCommandWithNoRequestOrResult, HandlerExecutionException>();
        }

        [Fact]
        public async Task CustomHandlerThrowsCorrectExceptionAfter()
        {
            await AssertHandlerThrowsCorrectException<FailAfterCommandHandler,
                ExampleCommandWithNoRequestOrResult, HandlerExecutionException>();
        }

        
    }        

    
    public class CustomHandlerReThrowsCorrectExceptionTests : EasyCommandServiceCollectionExtensionsTestsBase
    {
        [Fact]
        public async Task CustomHandlerReThrowsCorrectException()
        {
            await AssertHandlerThrowsCorrectException<ExampleHandler, ExceptionCommand, NotImplementedException>();
        }
    }

    public class EasyCommandServiceCollectionExtensionsTestsBase
    {
        protected IServiceProvider CreateServiceProvider(Action<IServiceCollection> addServices)
        {
            var services = new WebHostBuilder().UseStartup<Startup>().ConfigureTestServices(addServices).Build().Services;

            return services;
        }

        protected async Task AssertHandlerThrowsCorrectException<THandler, TCommand, TException>()
            where THandler : IAsyncAspCommandHandler
            where TCommand : AsyncAspCommandNoRequestNoResult
            where TException : Exception
        {
            var services = CreateServiceProvider(
                   t => t.AddEasyCommand(c => c.AddControllerCommandHandler<THandler>()));
            ControllerBaseExtensions.RegisterScope(services.CreateScope());

            var result = ControllerBaseExtensions.ExecuteAsync(null, services.GetRequiredService<TCommand>());

            await Assert.ThrowsAsync<TException>(async () => await result);
        }
    }


}
