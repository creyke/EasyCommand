using System;
using System.Threading.Tasks;

namespace EasyCommand.AspNetCore.Tests.Handler
{
    class FailAfterCommandHandler : IAsyncAspCommandHandler
    {
        public Task AfterExecutionAsync<TResponse>(Type command, TResponse response)
        {
            throw new NotImplementedException();
        }

        public Task BeforeExecutionAsync<TRequest>(Type command, TRequest request)
        {
            return Task.CompletedTask;
        }

    }
}
