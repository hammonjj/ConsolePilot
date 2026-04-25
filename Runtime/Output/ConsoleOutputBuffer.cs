using System;
using System.Collections.Generic;

namespace ConsolePilot.Output
{
    public sealed class ConsoleOutputBuffer : IConsoleOutput
    {
        private readonly List<ConsoleOutputEntry> _entries = new List<ConsoleOutputEntry>();
        private int _maxEntries;

        public ConsoleOutputBuffer(int maxEntries)
        {
            MaxEntries = maxEntries;
        }

        public event Action<ConsoleOutputEntry> EntryAdded;

        public event Action Cleared;

        public int MaxEntries
        {
            get { return _maxEntries; }
            set
            {
                _maxEntries = Math.Max(1, value);
                TrimToMaxEntries();
            }
        }

        public IReadOnlyList<ConsoleOutputEntry> Entries
        {
            get { return _entries; }
        }

        public void Write(ConsoleOutputEntry entry)
        {
            _entries.Add(entry);
            TrimToMaxEntries();
            EntryAdded?.Invoke(entry);
        }

        public void Clear()
        {
            _entries.Clear();
            Cleared?.Invoke();
        }

        private void TrimToMaxEntries()
        {
            var overflowCount = _entries.Count - _maxEntries;

            if (overflowCount <= 0)
            {
                return;
            }

            _entries.RemoveRange(0, overflowCount);
        }
    }
}
