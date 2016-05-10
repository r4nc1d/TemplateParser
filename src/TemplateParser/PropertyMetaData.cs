using System;
using System.Collections.Generic;

namespace TemplateParser
{
    public class PropertyMetaData
    {
        public Type Type { get; set; }
        public object Value { get; set; }

        private static readonly Dictionary<Type, Func<PropertyMetaData, object>> SanitizeFunctions = new Dictionary
            <Type, Func<PropertyMetaData, object>>
        {
            {typeof (string), property => Sanitize(property, string.Empty)},
            {typeof (int), property => Sanitize(property, 0)},
            {typeof (int?), property => Sanitize(property, 0)},
            {typeof (long), property => Sanitize(property, 0)},
            {typeof (long?), property => Sanitize(property, 0)},
            {typeof (decimal), property => Sanitize(property, 0)},
            {typeof (float), property => Sanitize(property, 0)},
            {typeof (DateTime), property => Sanitize(property, DateTime.Now)}
        };

        public static object Sanitize(PropertyMetaData propertyMetaData, object defaultValue)
        {
            return propertyMetaData.Value ?? defaultValue;
        }

        public static object SanitizeProperty(PropertyMetaData propertyMetaData)
        {
            return SanitizeFunctions[propertyMetaData.Type](propertyMetaData);
        }
    }
}