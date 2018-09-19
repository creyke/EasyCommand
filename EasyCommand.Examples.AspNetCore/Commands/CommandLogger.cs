using System;
using System.Threading.Tasks;
using EasyCommand.AspNetCore;

namespace EasyCommand.Examples.AspNetCore.Commands
{
    public class CommandLogger : IRunBeforeCommand
    {
        public Task BeforeExecutionOfCommand()
        {
            Console.WriteLine("Running before");
            return Task.CompletedTask;
;        }
    }
}
