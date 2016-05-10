using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TemplateParser.Enum;
using TemplateParser.Extension;
using Match = TemplateParser.Enum.Match;

namespace TemplateParser
{
    public class TemplateParser
    {
        /// <summary>Template string associated with the instance</summary>
        public readonly string TemplateString;

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
        public string Render(IDictionary<string, PropertyMetaData> variables, Placeholder placeholder)
        {
            return Render(TemplateString, variables, placeholder);
        }

        /// <summary>Render the supplied template string using the supplied variable values</summary>
        /// <param name="templateString">The template string to render</param>
        /// <param name="variables">Variables that can be substituted in the template string</param>
        /// <param name="placeholder"></param>
        /// <returns>The rendered template string</returns>
        public static string Render(string templateString, IDictionary<string, PropertyMetaData> variables, Placeholder placeholder)
        {
            if (templateString == null)
                throw new ArgumentNullException(nameof(templateString));

            var pattern = placeholder.GetSearchPattern();

            return Regex.Replace(templateString, pattern, match =>
            {
                switch (match.Value[0])
                {
                    case (char)Match.Backslash:
                        if (SpecialCharacters.EscapeChars.ContainsKey(match.Value[1]))
                            return SpecialCharacters.EscapeChars[match.Value[1]];
                        break;
                    case (char)Match.CurlyBracket:
                    case (char)Match.SquareBracket:
                        if (variables.ContainsKey(match.Groups[1].Value))
                            return GetValue(variables, match.Groups[1].Value);
                        break;
                }
                return string.Empty;
            }, RegexOptions.IgnoreCase);
        }

        private static string GetValue(IDictionary<string, PropertyMetaData> placeholders, string key)
        {
            var returnValue = GetPropertyMetaData(placeholders, key);
            return PropertyMetaData.SanitizeProject(returnValue) == null ?
                string.Empty : PropertyMetaData.SanitizeProject(returnValue).ToString();
        }

        private static PropertyMetaData GetPropertyMetaData(IDictionary<string, PropertyMetaData> placeholders, string key)
        {
            PropertyMetaData returnValue;
            if (!placeholders.TryGetValue(key, out returnValue))
            {
                returnValue = new PropertyMetaData {Type = typeof (string), Value = string.Empty};
            }
            return returnValue;
        }
    }
}
