using System;
using System.Collections;
using ConsolePilot.Settings;
using ConsolePilot.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace ConsolePilot.Tests
{
    public sealed class ConsolePilotViewLayoutTests
    {
        private GameObject _documentObject;
        private PanelSettings _panelSettings;
        private ThemeStyleSheet _themeStyleSheet;

        [TearDown]
        public void TearDown()
        {
            if (_documentObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_documentObject);
                _documentObject = null;
            }

            if (_panelSettings != null)
            {
                UnityEngine.Object.DestroyImmediate(_panelSettings);
                _panelSettings = null;
            }

            if (_themeStyleSheet != null)
            {
                UnityEngine.Object.DestroyImmediate(_themeStyleSheet);
                _themeStyleSheet = null;
            }
        }

        [UnityTest]
        public IEnumerator CommandInput_TextElementKeepsVisibleHeight_WhenValueChanges()
        {
            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _themeStyleSheet = ScriptableObject.CreateInstance<ThemeStyleSheet>();
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.referenceResolution = new Vector2Int(800, 400);
            _panelSettings.themeStyleSheet = _themeStyleSheet;

            _documentObject = new GameObject("ConsolePilotViewLayoutTest");
            var document = _documentObject.AddComponent<UIDocument>();
            document.panelSettings = _panelSettings;

            yield return null;

            var root = document.rootVisualElement;
            root.style.width = 800f;
            root.style.height = 400f;
            root.styleSheets.Add(LoadThemeStyleSheet());

            var view = new ConsolePilotView(root, ConsoleRuntimeSettings.CreateDefault());
            view.SetOpen(true);

            yield return null;

            var inputField = root.Q<TextField>(ConsolePilotView.InputName);
            inputField.SetValueWithoutNotify("test command");
            inputField.cursorIndex = inputField.value.Length;
            inputField.selectIndex = inputField.value.Length;
            view.FocusInput();

            TextElement textElement = null;

            for (var frame = 0; frame < 10; frame++)
            {
                yield return null;

                textElement = FindTextElement(root.Q<TextField>(ConsolePilotView.InputName), view.InputText);

                if (textElement != null && textElement.worldBound.height > 0f)
                {
                    break;
                }
            }

            Assert.AreEqual("test command", view.InputText);
            Assert.NotNull(textElement);
            Assert.AreEqual("test command", textElement.text);
            Assert.Greater(textElement.resolvedStyle.height, 0f);
            Assert.Greater(textElement.worldBound.height, 0f);
        }

        private static StyleSheet LoadThemeStyleSheet()
        {
            const string packageThemePath = "Packages/com.consolepilot.debugconsole/Runtime/UI/USS/ConsolePilotTheme.uss";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(packageThemePath);

            if (styleSheet != null)
            {
                return styleSheet;
            }

            foreach (var guid in AssetDatabase.FindAssets("ConsolePilotTheme t:StyleSheet"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith("Runtime/UI/USS/ConsolePilotTheme.uss", StringComparison.Ordinal))
                {
                    styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

                    if (styleSheet != null)
                    {
                        return styleSheet;
                    }
                }
            }

            Assert.Fail("Could not load ConsolePilotTheme.uss for layout regression test.");
            return null;
        }

        private static TextElement FindTextElement(VisualElement root, string text)
        {
            if (root == null)
            {
                return null;
            }

            TextElement result = null;

            root.Query<TextElement>().ForEach(element =>
            {
                if (result == null && element.text == text)
                {
                    result = element;
                }
            });

            return result;
        }
    }
}
