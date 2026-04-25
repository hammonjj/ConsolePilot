using ConsolePilot.Commands;
using NUnit.Framework;

namespace ConsolePilot.Tests
{
    public sealed class ConsoleCommandParserTests
    {
        [Test]
        public void Parse_ReturnsEmpty_ForWhitespaceInput()
        {
            var parser = new ConsoleCommandParser();

            var result = parser.Parse("   ");

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.IsEmpty);
        }

        [Test]
        public void Parse_SplitsCommandAndArguments()
        {
            var parser = new ConsoleCommandParser();

            var result = parser.Parse("echo hello world");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("echo", result.Command.Name);
            Assert.AreEqual(2, result.Command.Arguments.Count);
            Assert.AreEqual("hello", result.Command.Arguments[0]);
            Assert.AreEqual("world", result.Command.Arguments[1]);
        }

        [Test]
        public void Parse_PreservesQuotedArguments()
        {
            var parser = new ConsoleCommandParser();

            var result = parser.Parse("echo \"hello world\" test");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Command.Arguments.Count);
            Assert.AreEqual("hello world", result.Command.Arguments[0]);
            Assert.AreEqual("test", result.Command.Arguments[1]);
        }

        [Test]
        public void Parse_Fails_ForUnmatchedQuote()
        {
            var parser = new ConsoleCommandParser();

            var result = parser.Parse("echo \"unfinished");

            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Error);
        }
    }
}
