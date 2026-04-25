using System;

namespace ConsolePilot.Settings
{
    public sealed class ConsoleRuntimeSettings
    {
        private int _maxOutputEntries;
        private float _initialOpacity;
        private float _consoleHeightPercent;
        private string _toggleBindingPath;

        public bool UseBuiltInToggleInput { get; set; }

        public string ToggleBindingPath
        {
            get { return _toggleBindingPath; }
            set { _toggleBindingPath = string.IsNullOrWhiteSpace(value) ? ConsolePilotSettings.DefaultToggleBindingPath : value; }
        }

        public int MaxOutputEntries
        {
            get { return _maxOutputEntries; }
            set { _maxOutputEntries = Math.Max(1, value); }
        }

        public bool OpenOnStart { get; set; }

        public float InitialOpacity
        {
            get { return _initialOpacity; }
            set { _initialOpacity = Clamp(value, 0.25f, 1f); }
        }

        public float ConsoleHeightPercent
        {
            get { return _consoleHeightPercent; }
            set { _consoleHeightPercent = Clamp(value, 0.15f, 0.95f); }
        }

        public static ConsoleRuntimeSettings CreateDefault()
        {
            return new ConsoleRuntimeSettings
            {
                UseBuiltInToggleInput = true,
                ToggleBindingPath = ConsolePilotSettings.DefaultToggleBindingPath,
                MaxOutputEntries = 200,
                OpenOnStart = false,
                InitialOpacity = 0.94f,
                ConsoleHeightPercent = 0.45f
            };
        }

        private static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
