using ConsolePilot.Commands;
using ConsolePilot.Commands.BuiltIn;
using ConsolePilot.Dispatch;
using ConsolePilot.Input;
using ConsolePilot.Output;
using ConsolePilot.Settings;
using ConsolePilot.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace ConsolePilot
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class ConsolePilotRuntime : MonoBehaviour
    {
        [SerializeField] private ConsolePilotSettings _settings;
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        [SerializeField] private StyleSheet _themeStyleSheet;
        [SerializeField] private MonoBehaviour _eventDispatcherComponent;
        [SerializeField] private bool _dontDestroyOnLoad;

        private ConsoleCommandRegistry _commands;
        private ConsoleCommandExecutor _executor;
        private ConsoleOutputBuffer _output;
        private IConsoleEventDispatcher _events;
        private ConsoleRuntimeSettings _runtimeSettings;
        private InputSystemHotkeyController _hotkey;
        private ConsolePilotPresenter _presenter;

        public IConsoleCommandRegistry Commands
        {
            get { return _commands; }
        }

        public ConsoleOutputBuffer Output
        {
            get { return _output; }
        }

        public IConsoleEventDispatcher Events
        {
            get { return _events; }
        }

        public ConsoleRuntimeSettings Settings
        {
            get { return _runtimeSettings; }
        }

        public bool IsOpen
        {
            get { return _presenter != null && _presenter.IsOpen; }
        }

        public bool RegisterCommand(IConsoleCommand command, out string error)
        {
            if (_commands == null)
            {
                error = "ConsolePilotRuntime has not initialized yet.";
                return false;
            }

            return _commands.Register(command, out error);
        }

        public CommandResult ExecuteCommand(string input)
        {
            if (_executor == null)
            {
                return CommandResult.Fail("ConsolePilotRuntime has not initialized yet.");
            }

            return _executor.Execute(input);
        }

        public void SetEventDispatcher(IConsoleEventDispatcher eventDispatcher)
        {
            _events = eventDispatcher ?? new LocalConsoleEventDispatcher();
            _executor?.SetEventDispatcher(_events);
        }

        public void Open()
        {
            _presenter?.Open();
        }

        public void Close()
        {
            _presenter?.Close();
        }

        public void Toggle()
        {
            _presenter?.Toggle();
        }

        private void Awake()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            _runtimeSettings = _settings != null
                ? _settings.CreateRuntimeSettings()
                : ConsoleRuntimeSettings.CreateDefault();

            _commands = new ConsoleCommandRegistry();
            _output = new ConsoleOutputBuffer(_runtimeSettings.MaxOutputEntries);
            _events = ResolveEventDispatcher();

            var parser = new ConsoleCommandParser();
            _executor = new ConsoleCommandExecutor(parser, _commands, _output, _events, _runtimeSettings);

            RegisterBuiltInCommands();
            RegisterCommandProviders();
            BuildUi();

            _hotkey = new InputSystemHotkeyController(_runtimeSettings.ToggleBindingPath);
            _presenter = new ConsolePilotPresenter(CreateView(), _executor, _output, _hotkey, _runtimeSettings);
            _presenter.Initialize();
            _output.Write(ConsoleOutputEntry.Create("ConsolePilot ready. Type 'help' for commands.", ConsoleOutputLevel.System));
        }

        private void OnEnable()
        {
            _hotkey?.Enable();
        }

        private void Update()
        {
            _presenter?.Tick();
        }

        private void OnDisable()
        {
            _hotkey?.Disable();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            _hotkey?.Dispose();
        }

        private IConsoleEventDispatcher ResolveEventDispatcher()
        {
            if (_eventDispatcherComponent is IConsoleEventDispatcher configuredDispatcher)
            {
                return configuredDispatcher;
            }

            foreach (var component in GetComponents<MonoBehaviour>())
            {
                if (component == this)
                {
                    continue;
                }

                if (component is IConsoleEventDispatcher dispatcher)
                {
                    return dispatcher;
                }
            }

            return new LocalConsoleEventDispatcher();
        }

        private void RegisterBuiltInCommands()
        {
            RegisterOrLog(new HelpCommand(_commands));
            RegisterOrLog(new ClearCommand());
            RegisterOrLog(new EchoCommand());
        }

        private void RegisterCommandProviders()
        {
            foreach (var component in GetComponents<MonoBehaviour>())
            {
                if (component == this)
                {
                    continue;
                }

                if (component is IConsoleCommandProvider provider)
                {
                    provider.RegisterCommands(_commands);
                }
            }
        }

        private void RegisterOrLog(IConsoleCommand command)
        {
            if (_commands.Register(command, out var error))
            {
                return;
            }

            Debug.LogWarning($"ConsolePilot failed to register command '{command?.Descriptor?.Name}': {error}", this);
        }

        private void BuildUi()
        {
            _uiDocument = _uiDocument != null ? _uiDocument : GetComponent<UIDocument>();

            if (_uiDocument == null)
            {
                Debug.LogError("ConsolePilotRuntime requires a UIDocument.", this);
                return;
            }

            var root = _uiDocument.rootVisualElement;
            var visualTree = _visualTreeAsset != null ? _visualTreeAsset : _settings != null ? _settings.ConsoleVisualTree : null;

            if (visualTree != null && root.Q<VisualElement>(ConsolePilotView.RootName) == null)
            {
                visualTree.CloneTree(root);
            }

            var styleSheet = _themeStyleSheet != null ? _themeStyleSheet : _settings != null ? _settings.ThemeStyleSheet : null;

            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
        }

        private ConsolePilotView CreateView()
        {
            return new ConsolePilotView(_uiDocument.rootVisualElement, _runtimeSettings);
        }
    }
}
