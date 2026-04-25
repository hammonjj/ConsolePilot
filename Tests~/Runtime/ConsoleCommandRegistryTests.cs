using System.Collections.Generic;
using ConsolePilot.Commands;
using ConsolePilot.Core;
using NUnit.Framework;

namespace ConsolePilot.Tests
{
    public sealed class ConsoleCommandRegistryTests
    {
        [Test]
        public void Register_AllowsLookupByNameAndAlias()
        {
            var registry = new ConsoleCommandRegistry();

            var registered = registry.Register(new TestCommand("teleport", new[] { "tp" }), out var error);

            Assert.IsTrue(registered, error);
            Assert.IsTrue(registry.TryGet("teleport", out _));
            Assert.IsTrue(registry.TryGet("tp", out _));
        }

        [Test]
        public void Register_RejectsDuplicateAlias()
        {
            var registry = new ConsoleCommandRegistry();
            registry.Register(new TestCommand("teleport", new[] { "tp" }), out _);

            var registered = registry.Register(new TestCommand("team_points", new[] { "tp" }), out var error);

            Assert.IsFalse(registered);
            Assert.IsNotEmpty(error);
        }

        [Test]
        public void Unregister_RemovesNameAndAliases()
        {
            var registry = new ConsoleCommandRegistry();
            registry.Register(new TestCommand("teleport", new[] { "tp" }), out _);

            var removed = registry.Unregister("tp");

            Assert.IsTrue(removed);
            Assert.IsFalse(registry.TryGet("teleport", out _));
            Assert.IsFalse(registry.TryGet("tp", out _));
        }

        private sealed class TestCommand : IConsoleCommand
        {
            public TestCommand(string name, IReadOnlyList<string> aliases)
            {
                Descriptor = new CommandDescriptor(name, "Test command.", name, aliases);
            }

            public CommandDescriptor Descriptor { get; }

            public CommandResult Execute(CommandContext context, IReadOnlyList<string> arguments)
            {
                return CommandResult.Ok();
            }
        }
    }
}
