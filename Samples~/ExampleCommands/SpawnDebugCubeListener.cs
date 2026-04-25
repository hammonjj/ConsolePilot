using ConsolePilot.Dispatch;
using UnityEngine;

namespace ConsolePilot.Samples.ExampleCommands
{
    public sealed class SpawnDebugCubeListener : MonoBehaviour
    {
        [SerializeField] private ConsolePilotRuntime _consolePilot;

        private IConsoleSubscription _subscription;

        private void Start()
        {
            _consolePilot = _consolePilot != null ? _consolePilot : FindConsolePilotRuntime();

            if (_consolePilot == null)
            {
                Debug.LogWarning("SpawnDebugCubeListener could not find a ConsolePilotRuntime.", this);
                return;
            }

            _subscription = _consolePilot.Events.Subscribe<SpawnDebugCubeRequested>(OnSpawnDebugCubeRequested);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void OnSpawnDebugCubeRequested(SpawnDebugCubeRequested request)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "ConsolePilot Debug Cube";
            cube.transform.position = request.Position;
        }

        private static ConsolePilotRuntime FindConsolePilotRuntime()
        {
#if UNITY_2022_2_OR_NEWER
            return FindFirstObjectByType<ConsolePilotRuntime>();
#else
            return FindObjectOfType<ConsolePilotRuntime>();
#endif
        }
    }
}
