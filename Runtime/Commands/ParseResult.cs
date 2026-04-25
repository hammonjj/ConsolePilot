namespace ConsolePilot.Commands
{
    public readonly struct ParseResult
    {
        private ParseResult(bool success, bool isEmpty, ParsedCommand command, string error)
        {
            Success = success;
            IsEmpty = isEmpty;
            Command = command;
            Error = error ?? string.Empty;
        }

        public bool Success { get; }

        public bool IsEmpty { get; }

        public ParsedCommand Command { get; }

        public string Error { get; }

        public static ParseResult Empty()
        {
            return new ParseResult(true, true, default, string.Empty);
        }

        public static ParseResult Parsed(ParsedCommand command)
        {
            return new ParseResult(true, false, command, string.Empty);
        }

        public static ParseResult Failed(string error)
        {
            return new ParseResult(false, false, default, error);
        }
    }
}
