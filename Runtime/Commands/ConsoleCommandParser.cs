using System.Collections.Generic;
using System.Text;

namespace ConsolePilot.Commands
{
    public sealed class ConsoleCommandParser : IConsoleCommandParser
    {
        public ParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return ParseResult.Empty();
            }

            var tokens = new List<string>();
            var currentToken = new StringBuilder();
            var isInQuotes = false;
            var isEscaping = false;

            for (var i = 0; i < input.Length; i++)
            {
                var character = input[i];

                if (isEscaping)
                {
                    currentToken.Append(character);
                    isEscaping = false;
                    continue;
                }

                if (isInQuotes && character == '\\')
                {
                    isEscaping = true;
                    continue;
                }

                if (character == '"')
                {
                    isInQuotes = isInQuotes == false;
                    continue;
                }

                if (char.IsWhiteSpace(character) && isInQuotes == false)
                {
                    AddCurrentToken(tokens, currentToken);
                    continue;
                }

                currentToken.Append(character);
            }

            if (isEscaping)
            {
                currentToken.Append('\\');
            }

            if (isInQuotes)
            {
                return ParseResult.Failed("Unmatched quote in command input.");
            }

            AddCurrentToken(tokens, currentToken);

            if (tokens.Count == 0)
            {
                return ParseResult.Empty();
            }

            var commandName = tokens[0];
            tokens.RemoveAt(0);
            return ParseResult.Parsed(new ParsedCommand(commandName, tokens));
        }

        private static void AddCurrentToken(ICollection<string> tokens, StringBuilder currentToken)
        {
            if (currentToken.Length == 0)
            {
                return;
            }

            tokens.Add(currentToken.ToString());
            currentToken.Clear();
        }
    }
}
