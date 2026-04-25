namespace ConsolePilot
{
    public interface IConsolePilotOpenCloseTarget
    {
        bool IsOpen { get; }

        void SetOpen(bool isOpen);

        void Open();

        void Close();

        void Toggle();
    }
}
