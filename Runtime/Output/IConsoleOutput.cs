namespace ConsolePilot.Output
{
    public interface IConsoleOutput
    {
        void Write(ConsoleOutputEntry entry);

        void Clear();
    }
}
