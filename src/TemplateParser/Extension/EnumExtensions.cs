using System.ComponentModel;

namespace TemplateParser
{
    public static class EnumExtensions
    {
        public static string GetSearchPattern(this Placeholder value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}