namespace ConsolePilot.Commands
{
    public interface IConsoleCommandProvider
    {
        void RegisterCommands(IConsoleCommandRegistry registry);
    }
}
