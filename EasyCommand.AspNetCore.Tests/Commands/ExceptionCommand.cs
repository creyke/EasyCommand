using System;
using System.Threading.Tasks;

namespace EasyCommand.AspNetCore.Tests.Commands
{
    class ExceptionCommand : AsyncAspCommandNoRequestNoResult
    {
        protected override Task<EmptyCommandResult> ExecuteCommandAsync(EmptyCommandRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
