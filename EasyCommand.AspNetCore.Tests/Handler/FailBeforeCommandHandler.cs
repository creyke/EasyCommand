using System;
using System.Threading.Tasks;

namespace EasyCommand.AspNetCore.Tests.Handler
{
    class FailBeforeCommandHandler : IAsyncAspCommandHandler
    {
        public Task AfterExecutionAsync<TResponse>(Type command, TResponse response)
        {
            return Task.CompletedTask;
        }

        public Task BeforeExecutionAsync<TRequest>(Type command, TRequest request)
        {
            throw new NotImplementedException();
        }

    }
    
}
