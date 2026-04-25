using ConsolePilot.Dispatch;
using NUnit.Framework;

namespace ConsolePilot.Tests
{
    public sealed class LocalConsoleEventDispatcherTests
    {
        [Test]
        public void Publish_NotifiesSubscribers()
        {
            var dispatcher = new LocalConsoleEventDispatcher();
            var received = 0;
            dispatcher.Subscribe<int>(value => received = value);

            dispatcher.Publish(42);

            Assert.AreEqual(42, received);
        }

        [Test]
        public void DisposeSubscription_Unsubscribes()
        {
            var dispatcher = new LocalConsoleEventDispatcher();
            var receivedCount = 0;
            var subscription = dispatcher.Subscribe<int>(_ => receivedCount++);

            subscription.Dispose();
            dispatcher.Publish(42);

            Assert.AreEqual(0, receivedCount);
        }
    }
}
