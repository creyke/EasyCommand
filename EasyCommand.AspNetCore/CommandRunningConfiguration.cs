using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCommand.AspNetCore
{
    public class CommandRunningConfiguration
    {
        internal List<Type> toRunBeforeCommands { get; set; }

        public CommandRunningConfiguration()
        {
            toRunBeforeCommands = new List<Type>();
        }

        public void RunBeforeCommand<T>() where T : IRunBeforeCommand
        {
            toRunBeforeCommands.Add(typeof(T));
        }
    }
}
