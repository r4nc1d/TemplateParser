using System.Collections.Generic;

namespace TemplateParser
{
    public static class SpecialCharacters
    {
        public static readonly Dictionary<char, string> EscapeChars = new Dictionary<char, string>
        {
            {'r', "\r"},
            {'n', "\n"},
            {'\\', "\\"},
            {'{', "{"},
        };
    }
}