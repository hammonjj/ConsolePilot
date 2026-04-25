namespace ConsolePilot.Commands
{
    public interface IConsoleCommandParser
    {
        ParseResult Parse(string input);
    }
}
