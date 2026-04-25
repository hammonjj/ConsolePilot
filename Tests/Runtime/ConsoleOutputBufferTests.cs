using ConsolePilot.Output;
using NUnit.Framework;

namespace ConsolePilot.Tests
{
    public sealed class ConsoleOutputBufferTests
    {
        [Test]
        public void Write_TrimsOldEntries_WhenMaxEntriesExceeded()
        {
            var buffer = new ConsoleOutputBuffer(2);

            buffer.Write(ConsoleOutputEntry.Create("One", ConsoleOutputLevel.Info));
            buffer.Write(ConsoleOutputEntry.Create("Two", ConsoleOutputLevel.Info));
            buffer.Write(ConsoleOutputEntry.Create("Three", ConsoleOutputLevel.Info));

            Assert.AreEqual(2, buffer.Entries.Count);
            Assert.AreEqual("Two", buffer.Entries[0].Message);
            Assert.AreEqual("Three", buffer.Entries[1].Message);
        }

        [Test]
        public void Clear_RemovesEntries()
        {
            var buffer = new ConsoleOutputBuffer(2);
            buffer.Write(ConsoleOutputEntry.Create("One", ConsoleOutputLevel.Info));

            buffer.Clear();

            Assert.AreEqual(0, buffer.Entries.Count);
        }
    }
}
