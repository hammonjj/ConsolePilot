using System;
using ConsolePilot.Output;
using ConsolePilot.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ConsolePilot.UI
{
    public sealed class ConsolePilotView
    {
        public const string RootName = "ConsolePilotRoot";
        public const string PanelName = "ConsolePilotPanel";
        public const string OutputName = "ConsolePilotOutput";
        public const string InputName = "ConsolePilotInput";
        public const string SettingsPanelName = "ConsolePilotSettingsPanel";
        public const string SettingsButtonName = "ConsolePilotSettingsButton";
        public const string CloseButtonName = "ConsolePilotCloseButton";
        public const string HotkeyLabelName = "ConsolePilotHotkeyLabel";
        public const string CaptureHotkeyButtonName = "ConsolePilotCaptureHotkeyButton";
        public const string ResetHotkeyButtonName = "ConsolePilotResetHotkeyButton";

        private readonly VisualElement _documentRoot;
        private VisualElement _root;
        private VisualElement _panel;
        private ScrollView _outputView;
        private TextField _inputField;
        private VisualElement _settingsPanel;
        private Button _settingsButton;
        private Button _closeButton;
        private Label _hotkeyLabel;
        private Button _captureHotkeyButton;
        private Button _resetHotkeyButton;
        private bool _settingsVisible;

        public ConsolePilotView(VisualElement documentRoot, ConsoleRuntimeSettings settings)
        {
            _documentRoot = documentRoot;
            ResolveElements();
            ApplySettings(settings);
            BindUiEvents();
        }

        public event Action<string> CommandSubmitted;

        public event Action CaptureHotkeyRequested;

        public event Action ResetHotkeyRequested;

        public event Action CloseRequested;

        public bool IsOpen { get; private set; }

        public string InputText
        {
            get { return _inputField.value ?? string.Empty; }
        }

        public void SetOpen(bool isOpen)
        {
            IsOpen = isOpen;
            _root.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;

            if (isOpen)
            {
                FocusInput();
            }
        }

        public void FocusInput()
        {
            _inputField.schedule.Execute(() =>
            {
                _inputField.Focus();
                ApplyCaretIndex();
            });
        }

        public void ApplySettings(ConsoleRuntimeSettings settings)
        {
            _panel.style.opacity = settings.InitialOpacity;
            _panel.style.height = Length.Percent(settings.ConsoleHeightPercent * 100f);
        }

        public void RenderEntries(System.Collections.Generic.IEnumerable<ConsoleOutputEntry> entries)
        {
            ClearEntries();

            foreach (var entry in entries)
            {
                AppendEntry(entry);
            }
        }

        public void AppendEntry(ConsoleOutputEntry entry)
        {
            var label = new Label(FormatEntry(entry));
            label.AddToClassList("console-pilot-output-entry");
            label.AddToClassList($"console-pilot-output-entry--{entry.Level.ToString().ToLowerInvariant()}");
            _outputView.Add(label);
            _outputView.ScrollTo(label);
        }

        public void ClearEntries()
        {
            _outputView.Clear();
        }

        public void SetHotkeyText(string displayName)
        {
            _hotkeyLabel.text = string.IsNullOrWhiteSpace(displayName) ? "Unassigned" : displayName;
        }

        public void SetCaptureMode(bool isCapturing)
        {
            _captureHotkeyButton.text = isCapturing ? "Press a key..." : "Capture";
            _captureHotkeyButton.SetEnabled(isCapturing == false);
            _resetHotkeyButton.SetEnabled(isCapturing == false);
        }

        public void InsertCharacter(char character)
        {
            var text = InputText;
            var caretIndex = Mathf.Clamp(_inputField.cursorIndex, 0, text.Length);
            var nextText = text.Insert(caretIndex, character.ToString());
            SetInputText(nextText, caretIndex + 1);
        }

        public void Backspace()
        {
            var text = InputText;
            var caretIndex = Mathf.Clamp(_inputField.cursorIndex, 0, text.Length);

            if (caretIndex <= 0)
            {
                return;
            }

            var nextText = text.Remove(caretIndex - 1, 1);
            SetInputText(nextText, caretIndex - 1);
        }

        public void Delete()
        {
            var text = InputText;
            var caretIndex = Mathf.Clamp(_inputField.cursorIndex, 0, text.Length);

            if (caretIndex >= text.Length)
            {
                return;
            }

            var nextText = text.Remove(caretIndex, 1);
            SetInputText(nextText, caretIndex);
        }

        public void MoveCaretLeft()
        {
            SetCaretIndex(_inputField.cursorIndex - 1);
        }

        public void MoveCaretRight()
        {
            SetCaretIndex(_inputField.cursorIndex + 1);
        }

        public void SubmitInput()
        {
            var commandText = InputText;
            SetInputText(string.Empty, 0);
            CommandSubmitted?.Invoke(commandText);
        }

        private static string FormatEntry(ConsoleOutputEntry entry)
        {
            return $"[{entry.Timestamp:HH:mm:ss}] {entry.Message}";
        }

        private void ResolveElements()
        {
            _root = _documentRoot.Q<VisualElement>(RootName);

            if (_root == null)
            {
                BuildFallbackUi();
            }

            _panel = _root.Q<VisualElement>(PanelName);
            _outputView = _root.Q<ScrollView>(OutputName);
            _inputField = _root.Q<TextField>(InputName);
            _settingsPanel = _root.Q<VisualElement>(SettingsPanelName);
            _settingsButton = _root.Q<Button>(SettingsButtonName);
            _closeButton = _root.Q<Button>(CloseButtonName);
            _hotkeyLabel = _root.Q<Label>(HotkeyLabelName);
            _captureHotkeyButton = _root.Q<Button>(CaptureHotkeyButtonName);
            _resetHotkeyButton = _root.Q<Button>(ResetHotkeyButtonName);
        }

        private void BindUiEvents()
        {
            _settingsButton.clicked += ToggleSettingsPanel;
            _closeButton.clicked += () => CloseRequested?.Invoke();
            _captureHotkeyButton.clicked += () => CaptureHotkeyRequested?.Invoke();
            _resetHotkeyButton.clicked += () => ResetHotkeyRequested?.Invoke();
            _inputField.isReadOnly = true;
            SetSettingsVisible(false);
        }

        private void ToggleSettingsPanel()
        {
            SetSettingsVisible(_settingsVisible == false);
        }

        private void SetInputText(string text, int caretIndex)
        {
            _inputField.SetValueWithoutNotify(text ?? string.Empty);
            SetCaretIndex(caretIndex);
            FocusInput();
        }

        private void SetCaretIndex(int caretIndex)
        {
            var clampedIndex = Mathf.Clamp(caretIndex, 0, InputText.Length);
            _inputField.cursorIndex = clampedIndex;
            _inputField.selectIndex = clampedIndex;
        }

        private void ApplyCaretIndex()
        {
            SetCaretIndex(_inputField.cursorIndex);
        }

        private void SetSettingsVisible(bool isVisible)
        {
            _settingsVisible = isVisible;
            _settingsPanel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _settingsButton.text = isVisible ? "Hide Settings" : "Settings";
        }

        private void BuildFallbackUi()
        {
            _root = new VisualElement { name = RootName };
            _root.AddToClassList("console-pilot-root");

            var panel = new VisualElement { name = PanelName };
            panel.AddToClassList("console-pilot-panel");
            _root.Add(panel);

            var toolbar = new VisualElement();
            toolbar.AddToClassList("console-pilot-toolbar");
            panel.Add(toolbar);

            var title = new Label("ConsolePilot");
            title.AddToClassList("console-pilot-title");
            toolbar.Add(title);

            var spacer = new VisualElement();
            spacer.AddToClassList("console-pilot-spacer");
            toolbar.Add(spacer);

            var settingsButton = new Button { name = SettingsButtonName, text = "Settings" };
            settingsButton.AddToClassList("console-pilot-button");
            toolbar.Add(settingsButton);

            var closeButton = new Button { name = CloseButtonName, text = "Close" };
            closeButton.AddToClassList("console-pilot-button");
            toolbar.Add(closeButton);

            var output = new ScrollView(ScrollViewMode.Vertical) { name = OutputName };
            output.AddToClassList("console-pilot-output");
            panel.Add(output);

            var inputRow = new VisualElement();
            inputRow.AddToClassList("console-pilot-input-row");
            panel.Add(inputRow);

            var prompt = new Label(">");
            prompt.AddToClassList("console-pilot-prompt");
            inputRow.Add(prompt);

            var input = new TextField { name = InputName };
            input.AddToClassList("console-pilot-input");
            inputRow.Add(input);

            var settingsPanel = new VisualElement { name = SettingsPanelName };
            settingsPanel.AddToClassList("console-pilot-settings-panel");
            panel.Add(settingsPanel);

            var settingsTitle = new Label("Settings");
            settingsTitle.AddToClassList("console-pilot-settings-title");
            settingsPanel.Add(settingsTitle);

            var hotkeyRow = new VisualElement();
            hotkeyRow.AddToClassList("console-pilot-settings-row");
            settingsPanel.Add(hotkeyRow);

            var hotkeyTitle = new Label("Toggle Hotkey");
            hotkeyTitle.AddToClassList("console-pilot-settings-label");
            hotkeyRow.Add(hotkeyTitle);

            var hotkeyLabel = new Label { name = HotkeyLabelName };
            hotkeyLabel.AddToClassList("console-pilot-hotkey-label");
            hotkeyRow.Add(hotkeyLabel);

            var captureButton = new Button { name = CaptureHotkeyButtonName, text = "Capture" };
            captureButton.AddToClassList("console-pilot-button");
            hotkeyRow.Add(captureButton);

            var resetButton = new Button { name = ResetHotkeyButtonName, text = "Reset" };
            resetButton.AddToClassList("console-pilot-button");
            hotkeyRow.Add(resetButton);

            _documentRoot.Add(_root);
        }
    }
}
