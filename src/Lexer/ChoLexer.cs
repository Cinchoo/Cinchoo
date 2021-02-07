using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Cinchoo.Core
{
    public interface IChoMatcher
    {
        /// <summary>
        /// Return the number of characters that this "regex" or equivalent
        /// matches.
        /// </summary>
        /// <param name="text">The text to be matched</param>
        /// <returns>The number of characters that matched</returns>
        int Match(string text);
    }

    sealed class ChoRegexMatcher : IChoMatcher
    {
        private readonly Regex regex;
        public ChoRegexMatcher(string regex)
        {
            this.regex = new Regex(string.Format("^{0}", regex));
        }

        public int Match(string text)
        {
            var m = regex.Match(text);
            return m.Success ? m.Length : 0;
        }

        public override string ToString()
        {
            return regex.ToString();
        }
    }

    public sealed class ChoLexer<T> : IDisposable
    {
        private readonly TextReader reader;
        private readonly ChoTokenDefinition<T>[] tokenDefinitions;
        private string lineRemaining;

        public ChoLexer(TextReader reader, ChoTokenDefinition<T>[] tokenDefinitions)
        {
            ChoGuard.ArgumentNotNull(reader, "Reader");
            ChoGuard.ArgumentNotNull(tokenDefinitions, "TokenDefinitions");

            this.reader = reader;
            this.tokenDefinitions = tokenDefinitions;
            nextLine();
        }

        private void nextLine()
        {
            do
            {
                lineRemaining = reader.ReadLine();
                ++LineNumber;
                Position = 0;
            } while (lineRemaining != null && lineRemaining.Length == 0);
        }

        public ChoTokenContent<T> Next()
        {
            if (lineRemaining == null)
                return null;
            
            string tokenContents;
            T token;

            foreach (var def in tokenDefinitions)
            {
                var matched = def.Matcher.Match(lineRemaining);
                if (matched > 0)
                {
                    Position += matched;
                    token = def.Token;
                    tokenContents = lineRemaining.Substring(0, matched);
                    lineRemaining = lineRemaining.Substring(matched);
                    if (lineRemaining.Length == 0)
                        nextLine();

                    return new ChoTokenContent<T>(token, tokenContents, def.TokenDefType);
                }
            }
            throw new Exception(string.Format("Unable to match against any tokens at line {0} position {1} \"{2}\"",
                                              LineNumber, Position, lineRemaining));
        }

        public ChoTokenContent<T> Peek()
        {
            if (lineRemaining == null)
                return null;
            foreach (var def in tokenDefinitions)
            {
                var matched = def.Matcher.Match(lineRemaining);
                if (matched > 0)
                    return new ChoTokenContent<T>(def.Token, lineRemaining.Substring(0, matched), def.TokenDefType);
            }

            return null;
        }

        public int LineNumber { get; private set; }

        public int Position { get; private set; }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
