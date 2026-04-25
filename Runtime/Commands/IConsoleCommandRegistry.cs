using System.Collections.Generic;

namespace ConsolePilot.Commands
{
    public interface IConsoleCommandRegistry
    {
        bool Register(IConsoleCommand command, out string error);

        bool Unregister(string nameOrAlias);

        bool TryGet(string nameOrAlias, out IConsoleCommand command);

        IReadOnlyList<IConsoleCommand> GetAll();
    }
}
