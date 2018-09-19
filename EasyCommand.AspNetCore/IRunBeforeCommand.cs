using System.Threading.Tasks;

namespace EasyCommand.AspNetCore
{
    public interface IRunBeforeCommand
    {
        Task BeforeExecutionOfCommand();
    }
}
