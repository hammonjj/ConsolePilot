using System.Collections.Generic;
using ConsolePilot.Commands;
using ConsolePilot.Commands.BuiltIn;
using ConsolePilot.Core;
using ConsolePilot.Dispatch;
using ConsolePilot.Output;
using ConsolePilot.Settings;
using NUnit.Framework;

namespace ConsolePilot.Tests
{
    public sealed class ConsoleCommandExecutorTests
    {
        [Test]
        public void Execute_WritesError_ForUnknownCommand()
        {
            var executor = CreateExecutor(new ConsoleCommandRegistry(), out var output);

            var result = executor.Execute("missing");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(ConsoleOutputLevel.Error, output.Entries[1].Level);
        }

        [Test]
        public void Execute_WritesCommandResult()
        {
            var registry = new ConsoleCommandRegistry();
            registry.Register(new TestCommand(), out _);
            var executor = CreateExecutor(registry, out var output);

            var result = executor.Execute("test");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Test result.", output.Entries[1].Message);
        }

        [Test]
        public void Execute_ClearCommand_ClearsOutput()
        {
            var registry = new ConsoleCommandRegistry();
            registry.Register(new ClearCommand(), out _);
            var executor = CreateExecutor(registry, out var output);
            output.Write(ConsoleOutputEntry.Create("Before", ConsoleOutputLevel.Info));

            executor.Execute("clear");

            Assert.AreEqual(0, output.Entries.Count);
        }

        private static ConsoleCommandExecutor CreateExecutor(IConsoleCommandRegistry registry, out ConsoleOutputBuffer output)
        {
            output = new ConsoleOutputBuffer(20);
            return new ConsoleCommandExecutor(
                new ConsoleCommandParser(),
                registry,
                output,
                new LocalConsoleEventDispatcher(),
                ConsoleRuntimeSettings.CreateDefault());
        }

        private sealed class TestCommand : IConsoleCommand
        {
            public TestCommand()
            {
                Descriptor = new CommandDescriptor("test", "Test command.", "test");
            }

            public CommandDescriptor Descriptor { get; }

            public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
            {
                return CommandResult.Info("Test result.");
            }
        }
    }
}
