using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TemplateParser
{
    public enum Placeholder
    {
        Brace,
        Bracket
    }
    public class TemplateParser
    {
        /// <summary>Template string associated with the instance</summary>
        public readonly string TemplateString;

        private static readonly Dictionary<char, string> EscapeChars = new Dictionary<char, string>
        {
            {'r', "\r"},
            {'n', "\n"},
            {'\\', "\\"},
            {'{', "{"},
        };

        /// <summary>Create a new instance with the specified template string</summary>
        /// <param name="templateString">Template string associated with the instance</param>
        public TemplateParser(string templateString)
        {
            if (templateString == null)
                throw new ArgumentNullException(nameof(templateString));

            TemplateString = templateString;
        }

        /// <summary>Render the template using the supplied variable values</summary>
        /// <param name="variables">Variables that can be substituted in the template string</param>
        /// <param name="placeholder"></param>
        /// <returns>The rendered template string</returns>
        public string Render(IDictionary<string, object> variables, Placeholder placeholder)
        {
            return Render(TemplateString, variables, placeholder);
        }

        /// <summary>Render the supplied template string using the supplied variable values</summary>
        /// <param name="templateString">The template string to render</param>
        /// <param name="variables">Variables that can be substituted in the template string</param>
        /// <param name="placeholder"></param>
        /// <returns>The rendered template string</returns>
        public static string Render(string templateString, IDictionary<string, object> variables, Placeholder placeholder = Placeholder.Brace)
        {
            if (templateString == null)
                throw new ArgumentNullException(nameof(templateString));

            var pattern = GetSearchPattern(placeholder);
            return Regex.Replace(templateString, pattern, match =>
            {
                switch (match.Value[0])
                {
                    case '\\':
                        if (EscapeChars.ContainsKey(match.Value[1]))
                            return EscapeChars[match.Value[1]];
                        break;
                    case '{':
                        if (variables.ContainsKey(match.Groups[1].Value))
                            return variables[match.Groups[1].Value].ToString();
                        break;
                    case '[':
                        if (variables.ContainsKey(match.Groups[1].Value))
                            return variables[match.Groups[1].Value].ToString();
                        break;
                }
                return "";
            }, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns the matching pattern
        /// </summary>
        /// <param name="placeholder">Placehoder for either the [Brace] or {Bracket}</param>
        /// <returns></returns>
        public static string GetSearchPattern(Placeholder placeholder)
        {
            string pattern;
            switch (placeholder)
            {
                case Placeholder.Brace:
                    pattern = @"\[([a-z0-9_.\-]+)\]";
                    break;
                case Placeholder.Bracket:
                    pattern = @"\{([a-z0-9_.\-]+)\}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(placeholder), placeholder, null);
            }
            return pattern;
        }
    }
}
