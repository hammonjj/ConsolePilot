using UnityEngine;
using UnityEngine.UIElements;

namespace ConsolePilot.Settings
{
    [CreateAssetMenu(fileName = "ConsolePilotSettings", menuName = "ConsolePilot/Settings")]
    public sealed class ConsolePilotSettings : ScriptableObject
    {
        public const string DefaultToggleBindingPath = "<Keyboard>/backquote";

        [SerializeField] private bool _useBuiltInToggleInput = true;
        [SerializeField] private string _toggleBindingPath = DefaultToggleBindingPath;
        [SerializeField] private int _maxOutputEntries = 200;
        [SerializeField] private bool _openOnStart;
        [SerializeField] private float _initialOpacity = 0.94f;
        [SerializeField] private float _consoleHeightPercent = 0.45f;
        [SerializeField] private VisualTreeAsset _consoleVisualTree;
        [SerializeField] private StyleSheet _themeStyleSheet;

        public bool UseBuiltInToggleInput
        {
            get { return _useBuiltInToggleInput; }
        }

        public string ToggleBindingPath
        {
            get { return string.IsNullOrWhiteSpace(_toggleBindingPath) ? DefaultToggleBindingPath : _toggleBindingPath; }
        }

        public int MaxOutputEntries
        {
            get { return Mathf.Max(1, _maxOutputEntries); }
        }

        public bool OpenOnStart
        {
            get { return _openOnStart; }
        }

        public float InitialOpacity
        {
            get { return Mathf.Clamp(_initialOpacity, 0.25f, 1f); }
        }

        public float ConsoleHeightPercent
        {
            get { return Mathf.Clamp(_consoleHeightPercent, 0.15f, 0.95f); }
        }

        public VisualTreeAsset ConsoleVisualTree
        {
            get { return _consoleVisualTree; }
        }

        public StyleSheet ThemeStyleSheet
        {
            get { return _themeStyleSheet; }
        }

        public ConsoleRuntimeSettings CreateRuntimeSettings()
        {
            return new ConsoleRuntimeSettings
            {
                UseBuiltInToggleInput = UseBuiltInToggleInput,
                ToggleBindingPath = ToggleBindingPath,
                MaxOutputEntries = MaxOutputEntries,
                OpenOnStart = OpenOnStart,
                InitialOpacity = InitialOpacity,
                ConsoleHeightPercent = ConsoleHeightPercent
            };
        }
    }
}
